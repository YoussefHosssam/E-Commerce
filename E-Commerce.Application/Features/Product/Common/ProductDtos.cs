using E_Commerce.Application.Common.Dtos;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Features.Product.Common;

public sealed record ProductListItemDto
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = default!;
    public string CategorySlug { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? Brand { get; init; }
    public string Status { get; init; } = default!;
    public MoneyDto BasePrice { get; init; } = default!;
    public bool HasVariants { get; init; }
    public bool HasDiscount { get; init; }
    public MoneyDto? CompareAtPrice { get; init; }
    public Guid? DefaultVariantId { get; init; }
    public MoneyDto? MinPrice { get; init; }
    public MoneyDto? MaxPrice { get; init; }
    public bool IsActive { get; init; }
    public int VariantCount { get; init; }
}

public sealed record ProductVariantDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = default!;
    public string? Size { get; init; }
    public ColorDto Color { get; init; } = default!;
    public MoneyDto EffectivePrice { get; init; } = default!;
    public MoneyDto? VariantPriceOverride { get; init; }
    public int Stock { get; init; }
    public bool IsDefault { get; init; }
    public bool IsActive { get; init; }
}

public sealed record ProductDetailDto
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = default!;
    public string CategorySlug { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? Brand { get; init; }
    public string Status { get; init; } = default!;
    public MoneyDto BasePrice { get; init; } = default!;
    public bool HasVariants { get; init; }
    public bool HasDiscount { get; init; }
    public MoneyDto? CompareAtPrice { get; init; }
    public Guid? DefaultVariantId { get; init; }
    public MoneyDto? MinPrice { get; init; }
    public MoneyDto? MaxPrice { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public IReadOnlyCollection<ProductVariantDto> Variants { get; init; } = [];
}
