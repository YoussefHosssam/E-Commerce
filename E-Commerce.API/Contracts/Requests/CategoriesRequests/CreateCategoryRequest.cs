
namespace E_Commerce.API.Contracts.Requests.CategoriesRequests
{
    public sealed record CreateCategoryRequest(Guid? ParentId, string Slug, int SortOrder, bool IsActive = true);
}
