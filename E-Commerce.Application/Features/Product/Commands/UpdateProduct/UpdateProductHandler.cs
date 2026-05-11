using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Product.Commands.UpdateProduct;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ProductDetailDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.Id, true, cancellationToken);
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

        if (request.IsActive)
        {
            product.Activate(now);
        }
        else
        {
            product.Deactivate(now);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        var updatedProduct = await _uow.Products.GetByIdWithDetailsAsync(product.Id, false, cancellationToken) ?? product;
        return Result<ProductDetailDto>.Success(_mapper.Map<ProductDetailDto>(updatedProduct));
    }
}

