using E_Commerce.Application.Common.Dtos;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Variant.Common
{
    public sealed record VariantListItemDto
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductSlug { get; init; } = default!;
        public string Sku { get; init; } = default!;
        public string? Size { get; init; }
        public ColorDto Color { get; init; } = default!;
        public MoneyDto EffectivePrice { get; init; } = default!;
        public MoneyDto? VariantPriceOverride { get; init; }
        public int Stock { get; init; }
        public bool IsDefault { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record CartVariantDto
    {
        public Guid Id { get; init; }
        public string Sku { get; init; } = default!;
        public string? Size { get; init; }
        public ColorDto Color { get; init; } = default!;
        public MoneyDto EffectivePrice { get; init; } = default!;
        public MoneyDto? VariantPriceOverride { get; init; }
        public bool IsDefault { get; init; }
    }

    public sealed record VariantDetailDto
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string ProductSlug { get; init; } = default!;
        public string Sku { get; init; } = default!;
        public string? Size { get; init; }
        public ColorDto Color { get; init; } = default!;
        public MoneyDto EffectivePrice { get; init; } = default!;
        public MoneyDto? VariantPriceOverride { get; init; }
        public int Stock { get; init; }
        public bool IsDefault { get; init; }
        public bool IsActive { get; init; }
    }

    public sealed record VariantSnapshot
    {
        public Guid VariantId { get; init; }
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string? Sku { get; init; }
        public string? Size { get; init; }
        public ColorDto? Color { get; init; }
        public decimal UnitPrice { get; init; }
        public string CurrencyCode { get; init; } = "EGP";
    }
}
