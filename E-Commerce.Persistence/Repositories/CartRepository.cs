using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Features.Cart.Common;
using E_Commerce.Application.Features.Checkout.Common;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

internal class CartRepository : GenericRepository<Cart>, ICartRepository
{
    private readonly DbSet<Cart> _carts;

    public CartRepository(EcommerceContext ctx) : base(ctx)
    {
        _carts = ctx.Carts;
    }

    public async Task<Cart?> GetCartWithItemsByToken(string token, CancellationToken ctn)
    {
        return await _carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Variant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(x => x.AnonymousToken == token && x.Status == CartStatus.Active, ctn);
    }

    public async Task<Cart?> GetCartWithItemsByUserId(Guid id, CancellationToken ctn)
    {
        return await _carts
            .Include(c => c.Items)
            .ThenInclude(ci => ci.Variant)
            .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(x => x.UserId == id && x.Status == CartStatus.Active, ctn);
    }

    public async Task<CartSummaryDTO?> GetCartSummaryDtoByTokenAsync(string token, CancellationToken ctn)
    {
        return await ProjectCartSummary(_carts.AsNoTracking().Where(x => x.AnonymousToken == token && x.Status == CartStatus.Active))
            .FirstOrDefaultAsync(ctn);
    }

    public async Task<CartSummaryDTO?> GetCartSummaryDtoByUserIdAsync(Guid id, CancellationToken ctn)
    {
        return await ProjectCartSummary(_carts.AsNoTracking().Where(x => x.UserId == id && x.Status == CartStatus.Active))
            .FirstOrDefaultAsync(ctn);
    }

    public async Task<CheckoutSummaryDto?> GetCheckoutSummaryDtoByUserIdAsync(Guid id, CancellationToken ctn)
    {
        return await _carts
            .AsNoTracking()
            .Where(x => x.UserId == id && x.Status == CartStatus.Active)
            .Select(x => new CheckoutSummaryDto
            {
                Items = x.Items
                    .OrderBy(i => i.AddedAt)
                    .Select(i => new CheckoutItemDto
                    {
                        CartItemId = i.Id,
                        VariantId = i.VariantId,
                        Sku = i.Variant.Sku,
                        Size = i.Variant.Size,
                        Color = i.Variant.Color.Name,
                        ProductName = i.Variant.Product.Slug.Value,
                        Quantity = i.Quantity,
                        UnitPrice = i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount,
                        LineTotal = i.Quantity * (i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount),
                        Currency = i.Variant.Price == null ? i.Variant.Product.BasePrice.Currency.Value : i.Variant.Price.Currency.Value,
                        ImageUrl = i.Variant.Images
                            .Where(img => img.ProcessingStatus == ImageProcessingStatus.Uploaded)
                            .OrderByDescending(img => img.IsPrimary)
                            .ThenBy(img => img.SortOrder)
                            .Select(img => img.Url)
                            .FirstOrDefault()
                            ?? i.Variant.Product.Images
                                .Where(img => img.ProcessingStatus == ImageProcessingStatus.Uploaded)
                                .OrderByDescending(img => img.IsPrimary)
                                .ThenBy(img => img.SortOrder)
                                .Select(img => img.Url)
                                .FirstOrDefault()
                    })
                    .ToList(),
                TotalItems = x.Items.Count,
                TotalQuantity = x.Items.Sum(i => i.Quantity),
                Subtotal = x.Items.Sum(i => i.Quantity * (i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount)),
                ShippingFee = 0m,
                Total = x.Items.Sum(i => i.Quantity * (i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount)),
                Currency = x.Items
                    .Select(i => i.Variant.Price == null ? i.Variant.Product.BasePrice.Currency.Value : i.Variant.Price.Currency.Value)
                    .FirstOrDefault() ?? "EGP"
            })
            .FirstOrDefaultAsync(ctn);
    }

    private static IQueryable<CartSummaryDTO> ProjectCartSummary(IQueryable<Cart> query)
    {
        return query.Select(x => new CartSummaryDTO(
            x.Id,
            x.Items
                .OrderBy(i => i.AddedAt)
                .Select(i => new CartItemDTO(
                    i.Id,
                    i.Quantity,
                    new CartVariantDto
                    {
                        Id = i.Variant.Id,
                        Sku = i.Variant.Sku,
                        Size = i.Variant.Size,
                        Color = new ColorDto(i.Variant.Color.Name, i.Variant.Color.HexCode),
                        EffectivePrice = new MoneyDto(
                            i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount,
                            i.Variant.Price == null ? i.Variant.Product.BasePrice.Currency.Value : i.Variant.Price.Currency.Value),
                        VariantPriceOverride = i.Variant.Price == null
                            ? null
                            : new MoneyDto(i.Variant.Price.Amount, i.Variant.Price.Currency.Value),
                        IsDefault = i.Variant.IsDefault
                    }))
                .ToList(),
            x.Items.Sum(i => i.Quantity),
            x.Items.Sum(i => i.Quantity * (i.Variant.Price == null ? i.Variant.Product.BasePrice.Amount : i.Variant.Price.Amount)),
            x.AnonymousToken));
    }
}
