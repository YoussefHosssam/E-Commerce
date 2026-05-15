using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;

namespace E_Commerce.Application.Contracts.Persistence;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public Task<PagedResult<Category>> GetAllOrderedAsync(PageRequest page, CancellationToken ct);
    public Task<PagedResult<IReadOnlyCollection<Product>>?> GetProductsForCategory(Guid id, PageRequest page, CancellationToken ct);
    Task<Category?> GetByIdWithDetailsAsync(Guid id, bool asTracking, CancellationToken ct);
    Task<bool> SlugExistsAsync(Slug slug, Guid? excludedId, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, Guid? excludedId, CancellationToken ct);
    Task<bool> HasProductsAsync(Guid categoryId, CancellationToken ct);
    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct);
    Task<bool> IsDescendantAsync(Guid categoryId, Guid potentialParentId, CancellationToken ct);
}
