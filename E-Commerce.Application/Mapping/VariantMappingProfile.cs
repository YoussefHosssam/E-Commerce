using AutoMapper;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Mapping;

public class VariantMappingProfile : Profile
{
    public VariantMappingProfile()
    {
        CreateMap<Variant, VariantListItemDto>()
            .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug.Value))
            .ForMember(dest => dest.PriceOverride, opt => opt.MapFrom(src => src.PriceOverride != null ? MoneyDto.FromMoney(src.PriceOverride) : null));

        CreateMap<Variant, VariantDetailDto>()
            .ForMember(dest => dest.ProductSlug, opt => opt.MapFrom(src => src.Product.Slug.Value))
            .ForMember(dest => dest.PriceOverride, opt => opt.MapFrom(src => src.PriceOverride != null ? MoneyDto.FromMoney(src.PriceOverride) : null));

        CreateMap<Variant, CartVariantDto>()
            .ForMember(dest => dest.PriceOverride, opt => opt.MapFrom(src => src.PriceOverride != null ? MoneyDto.FromMoney(src.PriceOverride) : null));

        CreateMap<Variant, VariantSnapshot>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.VariantId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.GetPrice().Amount))
            .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.GetPrice().Currency.Value));
    }
}
