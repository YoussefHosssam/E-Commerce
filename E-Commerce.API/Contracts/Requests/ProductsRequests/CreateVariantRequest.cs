
namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public sealed record CreateVariantRequest(
        string Sku,
        string? Size,
        ColorRequest Color,
        int Stock,
        bool IsDefault,
        decimal? VariantPriceOverrideAmount = null,
        bool IsActive = true);
}
