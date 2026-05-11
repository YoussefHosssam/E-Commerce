using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using AutoMapper;

namespace E_Commerce.Application.Features.Product.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductDetailDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateProductHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
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
        var product = Domain.Entities.Product.Create(request.Name , request.CategoryId, slug, money, request.Brand, request.Status);

        if (!request.IsActive)
        {
            product.Deactivate(DateTimeOffset.UtcNow);
        }

        await _uow.Products.CreateAsync(product, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var createdProduct = await _uow.Products.GetByIdWithDetailsAsync(product.Id, false, cancellationToken) ?? product;
        return Result<ProductDetailDto>.Success(_mapper.Map<ProductDetailDto>(createdProduct));
    }
}

