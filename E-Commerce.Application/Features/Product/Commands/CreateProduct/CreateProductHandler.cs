using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Product.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IVariantService _variantService;

    public CreateProductHandler(IUnitOfWork uow, IVariantService variantService)
    {
        _uow = uow;
        _variantService = variantService;
    }

    public async Task<Result<ProductDetailDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            return Result<ProductDetailDto>.Fail(CategoryErrors.NotFound);
        }

        var slug = Slug.Create(request.Slug);
        if (await _uow.Products.SlugExistsAsync(slug, null, cancellationToken))
        {
            return Result<ProductDetailDto>.Fail(ProductErrors.SlugDuplicate);
        }

        var money = Money.Create(request.BasePriceAmount, CurrencyCode.Create(request.BasePriceCurrency));
        var product = Domain.Entities.Product.Create(request.Name , request.CategoryId, slug, money, request.HasVariants, request.Brand, request.Status);

        var variantsResult = await _variantService.CreateVariantsForProductAsync(product, request.Variants, cancellationToken);
        if (!variantsResult.IsSuccess)
            return Result<ProductDetailDto>.Fail(variantsResult.Error!);

        if (request.HasDiscount)
        {
            var compareAtPrice = Money.Create(request.CompareAtPriceAmount!.Value, CurrencyCode.Create(request.CompareAtPriceCurrency!));
            product.ApplyDiscount(compareAtPrice);
        }

        if (!request.IsActive)
        {
            product.Deactivate(DateTimeOffset.UtcNow);
        }

        await _uow.Products.CreateAsync(product, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var createdProduct = await _uow.Products.GetProductDetailsDtoAsync(product.Id, cancellationToken);
        return Result<ProductDetailDto>.Success(createdProduct!);
    }
}

