using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<PagedResult<Product>> GetPagedProducts(PageRequest pageRequest, CancellationToken ctn);
    Task<PagedResult<Product>> GetPagedProductsWithDetailsAsync(PageRequest pageRequest, CancellationToken ct);
    Task<Product?> GetByIdWithDetailsAsync(Guid id, bool asTracking, CancellationToken ct);
    Task<bool> SlugExistsAsync(Slug slug, Guid? excludedId, CancellationToken ct);
}
