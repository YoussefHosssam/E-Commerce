using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Product.Commands.UpdateProduct;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IVariantService _variantService;

    public UpdateProductHandler(IUnitOfWork uow, IVariantService variantService)
    {
        _uow = uow;
        _variantService = variantService;
    }

    public async Task<Result<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetAggregateByIdAsync(request.Id, true, cancellationToken);
        if (product is null)
        {
            return Result<ProductDetailDto>.Fail(ProductErrors.NotFound);
        }

        var category = await _uow.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result<ProductDetailDto>.Fail(CategoryErrors.NotFound);
        }

        var slug = Slug.Create(request.Slug);
        if (await _uow.Products.SlugExistsAsync(slug, product.Id, cancellationToken))
        {
            return Result<ProductDetailDto>.Fail(ProductErrors.SlugDuplicate);
        }

        var money = Money.Create(request.BasePriceAmount, CurrencyCode.Create(request.BasePriceCurrency));
        var now = DateTimeOffset.UtcNow;

        product.ChangeName(request.Name, now);
        product.ChangeCategory(request.CategoryId, now);
        product.ChangeSlug(slug, now);
        product.ChangeBrand(request.Brand, now);
        product.ChangeBasePrice(money, now);
        product.ChangeStatus(request.Status, now);
        if (request.Variants is { Count: > 0 })
        {
            foreach (var variant in product.Variants.Where(v => v.IsActive))
            {
                if (await _uow.Variants.IsVariantUsedInActiveCartsAsync(variant.Id, cancellationToken))
                    return Result<ProductDetailDto>.Fail(VariantErrors.CannotDeleteVariantUsedInCart);
            }

            product.ArchiveActiveVariantsForReplacement(now);
            product.ChangeVariantMode(request.HasVariants, now);

            var variantsResult = await _variantService.CreateVariantsForProductAsync(product, request.Variants, cancellationToken);
            if (!variantsResult.IsSuccess)
                return Result<ProductDetailDto>.Fail(variantsResult.Error!);
        }
        else
        {
            product.ChangeVariantMode(request.HasVariants, now);
        }

        if (request.HasDiscount)
        {
            var compareAtPrice = Money.Create(request.CompareAtPriceAmount!.Value, CurrencyCode.Create(request.CompareAtPriceCurrency!));
            product.ApplyDiscount(compareAtPrice);
        }
        else
        {
            product.RemoveDiscount();
        }

        if (request.IsActive)
        {
            product.Activate(now);
        }
        else
        {
            product.Deactivate(now);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        var updatedProduct = await _uow.Products.GetProductDetailsDtoAsync(product.Id, cancellationToken);
        return Result<ProductDetailDto>.Success(updatedProduct!);
    }
}

