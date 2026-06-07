namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
public sealed record UpdateVariantRequest(
    string Sku,
    string? Size,
    ColorRequest Color,
    bool? HasPriceOverride,
    decimal? VariantPriceOverrideAmount,
    bool IsDefault,
    bool IsActive = true);
}
