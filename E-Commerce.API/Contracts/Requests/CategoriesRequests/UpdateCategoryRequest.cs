
namespace E_Commerce.API.Contracts.Requests.CategoriesRequests
{
    public sealed record UpdateCategoryRequest(Guid? ParentId,string Name, string Slug, int SortOrder, bool IsActive = true);
}
