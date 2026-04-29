namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public sealed record UpdateVariantRequest(string Sku, string? Size, string? Color, decimal? PriceOverrideAmount, string? PriceOverrideCurrency, bool IsActive = true);
}
