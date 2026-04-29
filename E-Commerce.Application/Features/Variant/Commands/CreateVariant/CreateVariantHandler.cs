using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.CreateVariant;

public sealed class CreateVariantHandler : IRequestHandler<CreateVariantCommand, Result<VariantDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public CreateVariantHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<VariantDetailDto>> Handle(CreateVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
        {
            return Result<VariantDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.NotFound));
        }

        if (await _uow.Variants.SkuExistsAsync(request.Sku, null, cancellationToken))
        {
            return Result<VariantDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Variant.SkuDuplicate));
        }

        Money? priceOverride = null;
        if (request.PriceOverrideAmount.HasValue)
        {
            priceOverride = Money.Create(request.PriceOverrideAmount.Value, CurrencyCode.Create(request.PriceOverrideCurrency!));
        }

        var variant = product.AddVariant(request.Sku, request.Size, request.Color, priceOverride, DateTimeOffset.UtcNow);
        if (!request.IsActive)
        {
            variant.Deactivate();
        }

        await _uow.SaveChangesAsync(cancellationToken);
        var createdVariant = await _uow.Variants.GetByIdWithDetailsAsync(variant.Id, cancellationToken);
        return Result<VariantDetailDto>.Success((createdVariant ?? variant).ToDetailDto());
    }
}

