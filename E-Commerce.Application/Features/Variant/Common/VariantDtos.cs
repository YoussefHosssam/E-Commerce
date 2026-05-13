using E_Commerce.Application.Common.Dtos;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Variant.Common
{
    public sealed record VariantListItemDto(
        Guid Id,
        Guid ProductId,
        string ProductSlug,
        string Sku,
        string? Size,
        string? Color,
        MoneyDto? PriceOverride,
        bool IsActive);
    public sealed record CartVariantDto(
        Guid Id,
        string Sku,
        string? Size,
        string? Color,
        MoneyDto? PriceOverride);
    public sealed record VariantDetailDto(
        Guid Id,
        Guid ProductId,
        string ProductSlug,
        string Sku,
        string? Size,
        string? Color,
        MoneyDto? PriceOverride,
        bool IsActive);
    public sealed record VariantSnapshot
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; } = "EGP";
    }
}
