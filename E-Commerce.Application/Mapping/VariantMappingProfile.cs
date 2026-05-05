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
    }
}
