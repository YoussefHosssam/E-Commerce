using E_Commerce.Domain.Enums;


namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public sealed record CreateProductRequest(string Name , Guid CategoryId, string Slug, string? Brand, decimal BasePriceAmount, string BasePriceCurrency, ProductStatus Status = ProductStatus.Active, bool IsActive = true);
}
