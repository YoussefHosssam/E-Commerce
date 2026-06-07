using AutoMapper;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Mapping;

public class VariantMappingProfile : Profile
{
    public VariantMappingProfile()
    {
        CreateMap<Variant, VariantListItemDto>()
            .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug.Value))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => ColorDto.FromColor(src.Color)))
            .ForMember(dest => dest.EffectivePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.GetPrice())))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory.Available : 0))
            .ForMember(dest => dest.VariantPriceOverride, opt => opt.MapFrom(src => src.Price != null ? MoneyDto.FromMoney(src.Price) : null));

        CreateMap<Variant, VariantDetailDto>()
            .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug.Value))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => ColorDto.FromColor(src.Color)))
            .ForMember(dest => dest.EffectivePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.GetPrice())))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory.Available : 0))
            .ForMember(dest => dest.VariantPriceOverride, opt => opt.MapFrom(src => src.Price != null ? MoneyDto.FromMoney(src.Price) : null));

        CreateMap<Variant, CartVariantDto>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => ColorDto.FromColor(src.Color)))
            .ForMember(dest => dest.EffectivePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.GetPrice())))
            .ForMember(dest => dest.VariantPriceOverride, opt => opt.MapFrom(src => src.Price != null ? MoneyDto.FromMoney(src.Price) : null));

        CreateMap<Variant, ProductVariantDto>()
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => ColorDto.FromColor(src.Color)))
            .ForMember(dest => dest.EffectivePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.GetPrice())))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory.Available : 0))
            .ForMember(dest => dest.VariantPriceOverride, opt => opt.MapFrom(src => src.Price != null ? MoneyDto.FromMoney(src.Price) : null));

        CreateMap<Variant, VariantSnapshot>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.GetPrice().Amount))
            .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => ColorDto.FromColor(src.Color)))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.GetPrice().Currency.Value));
    }
}
