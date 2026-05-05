using AutoMapper;
using E_Commerce.Domain.Entities;
using CartEntity = E_Commerce.Domain.Entities.Cart;

namespace E_Commerce.Application.Features.Checkout.Common;

public sealed class CheckoutMappingProfile : Profile
{
    public CheckoutMappingProfile()
    {
        CreateMap<CartItem, CheckoutItemDto>()
            .ForMember(d => d.CartItemId,
                opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.VariantId,
                opt => opt.MapFrom(s => s.VariantId))
            .ForMember(d => d.Sku,
                opt => opt.MapFrom(s => s.Variant.Sku))
            .ForMember(d => d.Size,
                opt => opt.MapFrom(s => s.Variant.Size))
            .ForMember(d => d.Color,
                opt => opt.MapFrom(s => s.Variant.Color))
            .ForMember(d => d.ProductName,
                opt => opt.MapFrom(s => s.Variant.Product.Slug.Value))
            .ForMember(d => d.Quantity,
                opt => opt.MapFrom(s => s.Quantity))
            .ForMember(d => d.UnitPrice,
                opt => opt.MapFrom(s =>
                    s.Variant.PriceOverride != null
                        ? s.Variant.PriceOverride.Amount
                        : s.Variant.Product.BasePrice.Amount))
            .ForMember(d => d.Currency,
                opt => opt.MapFrom(s =>
                    s.Variant.PriceOverride != null
                        ? s.Variant.PriceOverride.Currency.Value
                        : s.Variant.Product.BasePrice.Currency.Value))
            .ForMember(d => d.LineTotal,
                opt => opt.MapFrom(s =>
                    s.Quantity *
                    (s.Variant.PriceOverride != null
                        ? s.Variant.PriceOverride.Amount
                        : s.Variant.Product.BasePrice.Amount)))
            .ForMember(d => d.ImageUrl,
                opt => opt.MapFrom(s =>
                    s.Variant.Images.Any()
                        ? s.Variant.Images.First().Url
                        : s.Variant.Product.Images.Any()
                            ? s.Variant.Product.Images.First().Url
                            : null));

        CreateMap<CartEntity, CheckoutSummaryDto>()
            .ForCtorParam("Items",
                opt => opt.MapFrom(s => s.Items))
            .ForCtorParam("TotalItems",
                opt => opt.MapFrom(s => s.Items.Count))
            .ForCtorParam("TotalQuantity",
                opt => opt.MapFrom(s => s.Items.Sum(i => i.Quantity)))
            .ForCtorParam("Subtotal",
                opt => opt.MapFrom(s => s.Items.Sum(i =>
                    i.Quantity *
                    (i.Variant.PriceOverride != null
                        ? i.Variant.PriceOverride.Amount
                        : i.Variant.Product.BasePrice.Amount))))
            .ForCtorParam("ShippingFee",
                opt => opt.MapFrom(_ => 0m))
            .ForCtorParam("Total",
                opt => opt.MapFrom(s => s.Items.Sum(i =>
                    i.Quantity *
                    (i.Variant.PriceOverride != null
                        ? i.Variant.PriceOverride.Amount
                        : i.Variant.Product.BasePrice.Amount))))
            .ForCtorParam("Currency",
                opt => opt.MapFrom(s =>
                    s.Items.Any()
                        ? s.Items.First().Variant.PriceOverride != null
                            ? s.Items.First().Variant.PriceOverride!.Currency.Value
                            : s.Items.First().Variant.Product.BasePrice.Currency.Value
                        : "EGP"));
    }
}