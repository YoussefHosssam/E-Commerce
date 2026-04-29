using E_Commerce.Domain.Enums;


namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public sealed record CreateProductRequest(Guid CategoryId, string Slug, string? Brand, decimal BasePriceAmount, string BasePriceCurrency, ProductStatus Status, bool IsActive = true);
}
