using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System.Threading;

namespace E_Commerce.Application.Services;

public sealed class VariantService : IVariantService
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;
    private readonly VariantCreateDtoListValidator _createVariantListValidator;
    public VariantService(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
        _createVariantListValidator = new();
    }

    public async Task<Result<IReadOnlyList<Variant>>> CreateVariantsForProductAsync(
        Product product,
        IReadOnlyCollection<VariantCreateDto> variants,
        CancellationToken cancellationToken)
    {
        if (variants.Count == 0)
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.ProductMustHaveAtLeastOneVariant);

        if (!product.HasVariants && product.Variants.Any(v => v.IsActive))
            return Result<IReadOnlyList<Variant>>.Fail(ProductErrors.CannotAddVariantsToSimpleProduct);

        if (!product.HasVariants && variants.Count != 1)
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.SimpleProductMustHaveExactlyOneVariant);

        if (!product.HasVariants && !variants.Single().IsDefault)
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.ProductMustHaveDefaultVariant);

        var validations = await _createVariantListValidator.ValidateAsync(variants, cancellationToken);
        if (!validations.IsValid)
        {
            var error = validations.Errors.First();
            return Result<IReadOnlyList<Variant>>.Fail(error.GetError());
        }

        var requestedDefaults = variants.Count(v => v.IsDefault);
        var existingActiveDefault = product.Variants.Any(v => v.IsActive && v.IsDefault);

        if (requestedDefaults == 0 && !existingActiveDefault)
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.ProductMustHaveDefaultVariant);

        if (requestedDefaults > 1 || (requestedDefaults == 1 && existingActiveDefault))
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.DefaultVariantAlreadyExists);

        var duplicateSku = variants
            .Select(v => v.Sku)
            .GroupBy(v => v)
            .Any(g => g.Count() > 1);

        if (duplicateSku)
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.SkuDuplicate);

        foreach (var sku in variants.Select(v => v.Sku))
        {
            if (await _uow.Variants.SkuExistsAsync(sku, null, cancellationToken))
                return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.SkuDuplicate);
        }

        if (HasDuplicateOptions(product, variants))
            return Result<IReadOnlyList<Variant>>.Fail(VariantErrors.DuplicateVariantOptions);

        var now = DateTimeOffset.UtcNow;
        var actorUserId = _userAccessor.GetRequiredUserId();
        var created = new List<Variant>();

        foreach (var item in variants)
        {
            var variant = await CreateVariant(item, product, actorUserId, now, cancellationToken);
            created.Add(variant);
        }

        return Result<IReadOnlyList<Variant>>.Success(created);
    }

    private async Task<Variant> CreateVariant(VariantCreateDto item , Product product , Guid actorUserId, DateTimeOffset now , CancellationToken cancellationToken)
    {
        var color = Color.Create(item.Color!.Name, item.Color.HexCode);
        var price = item.VariantPriceOverrideAmount.HasValue
            ? Money.Create(item.VariantPriceOverrideAmount.Value, product.BasePrice.Currency)
            : null;

        var variant = product.AddVariant(
            item.Sku,
            item.Size,
            color,
            price,
            item.IsDefault,
            now);

        if (!item.IsActive)
            variant.Deactivate();

        var inventory = Inventory.Create(variant.Id, item.Stock, now);
        await _uow.Inventories.CreateAsync(inventory, cancellationToken);

        var movement = StockMovement.Create(
            variant.Id,
            StockMovementType.InitialStock,
            item.Stock,
            "New Variant has been added.",
            variant.Id,
            actorUserId,
            now);

        await _uow.StockMovements.CreateAsync(movement, cancellationToken);
        return variant;
    }
    private static bool HasDuplicateOptions(Product product, IReadOnlyCollection<VariantCreateDto> variants)
    {
        var existingOptions = product.Variants
            .Where(v => v.IsActive)
            .Select(v => BuildOptionKey(v.Size, v.Color.HexCode))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var requestOptions = variants
            .Select(v => BuildOptionKey(v.Size, v.Color?.HexCode))
            .ToList();

        return requestOptions.GroupBy(x => x, StringComparer.OrdinalIgnoreCase).Any(g => g.Count() > 1)
            || requestOptions.Any(existingOptions.Contains);
    }

    private static string BuildOptionKey(string? size, string? colorHexCode)
        => $"{size?.Trim().ToUpperInvariant() ?? string.Empty}|{colorHexCode?.Trim().ToUpperInvariant() ?? string.Empty}";
}
