using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

public sealed class UpdateVariantHandler : IRequestHandler<UpdateVariantCommand, Result<VariantDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateVariantHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<VariantDetailDto>> Handle(UpdateVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
        {
            return Result<VariantDetailDto>.Fail(ProductErrors.NotFound);
        }

        if (!product.Variants.Any(x => x.Id == request.VariantId))
        {
            return Result<VariantDetailDto>.Fail(VariantErrors.NotFound);
        }

        if (await _uow.Variants.SkuExistsAsync(request.Sku, request.VariantId, cancellationToken))
        {
            return Result<VariantDetailDto>.Fail(VariantErrors.SkuDuplicate);
        }

        var color = Color.Create(request.Color!.Name, request.Color.HexCode);
        Money? price = null;
        var updatePrice = request.HasPriceOverride.HasValue;
        if (request.HasPriceOverride == true)
        {
            price = Money.Create(request.VariantPriceOverrideAmount!.Value, product.BasePrice.Currency);
        }

        var hasDuplicateOptions = product.Variants.Any(x =>
            x.Id != request.VariantId
            && x.IsActive
            && string.Equals(x.Size ?? string.Empty, request.Size?.Trim() ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Color.HexCode, color.HexCode, StringComparison.OrdinalIgnoreCase));

        if (hasDuplicateOptions)
            return Result<VariantDetailDto>.Fail(VariantErrors.DuplicateVariantOptions);

        product.UpdateVariant(request.VariantId, request.Sku, request.Size, color, price, updatePrice, request.IsDefault, request.IsActive, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        var updatedVariant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, cancellationToken);
        return Result<VariantDetailDto>.Success(_mapper.Map<VariantDetailDto>(updatedVariant!));
    }
}

