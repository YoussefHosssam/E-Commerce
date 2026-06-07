namespace E_Commerce.API.Contracts.Requests.ProductsRequests;

public sealed record ProductVariantRequest(
    string Sku,
    string? Size,
    ColorRequest Color,
    int Stock,
    bool IsDefault,
    bool IsActive = true,
    decimal? VariantPriceOverrideAmount = null
);
