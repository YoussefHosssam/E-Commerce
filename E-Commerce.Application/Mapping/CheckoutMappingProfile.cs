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
                opt => opt.MapFrom(s => s.Variant.Color.Name))
            .ForMember(d => d.ProductName,
                opt => opt.MapFrom(s => s.Variant.Product.Slug.Value))
            .ForMember(d => d.Quantity,
                opt => opt.MapFrom(s => s.Quantity))
            .ForMember(d => d.UnitPrice,
                opt => opt.MapFrom(s => s.Variant.GetPrice().Amount))
            .ForMember(d => d.Currency,
                opt => opt.MapFrom(s => s.Variant.GetPrice().Currency.Value))
            .ForMember(d => d.LineTotal,
                opt => opt.MapFrom(s => s.Quantity * s.Variant.GetPrice().Amount))
            .ForMember(d => d.ImageUrl,
                opt => opt.MapFrom(s =>
                    s.Variant.Images.Any()
                        ? s.Variant.Images.First().Url
                        : s.Variant.Product.Images.Any()
                            ? s.Variant.Product.Images.First().Url
                            : null));

        CreateMap<CartEntity, CheckoutSummaryDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items))
            .ForMember(d => d.TotalItems, opt => opt.MapFrom(s => s.Items.Count))
            .ForMember(d => d.TotalQuantity, opt => opt.MapFrom(s => s.Items.Sum(i => i.Quantity)))
            .ForMember(d => d.Subtotal, opt => opt.MapFrom(s => s.Items.Sum(i => i.Quantity * i.Variant.GetPrice().Amount)))
            .ForMember(d => d.ShippingFee, opt => opt.MapFrom(_ => 0m))
            .ForMember(d => d.Total, opt => opt.MapFrom(s => s.Items.Sum(i => i.Quantity * i.Variant.GetPrice().Amount)))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s =>
                s.Items.Any()
                    ? s.Items.First().Variant.GetPrice().Currency.Value
                    : "EGP"));

        CreateMap<UserAddress, CheckoutAddressDto>();
    }
}
