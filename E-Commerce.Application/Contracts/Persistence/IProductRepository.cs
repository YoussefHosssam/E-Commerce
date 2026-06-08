using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<PagedResult<Product>> GetPagedProducts(PageRequest pageRequest, CancellationToken ctn);
    Task<PagedResult<ProductListItemDto>> GetProductListItemDtosAsync(PageRequest pageRequest, CancellationToken ct);
    Task<ProductDetailDto?> GetProductDetailsDtoAsync(Guid id, CancellationToken ct);
    Task<Product?> GetAggregateByIdAsync(Guid id, bool asTracking, CancellationToken ct);
    Task<bool> SlugExistsAsync(Slug slug, Guid? excludedId, CancellationToken ct);
}
