using AutoMapper;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductListItemDto>()
            .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.Slug.Value))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.BasePrice)))
            .ForMember(dest => dest.VariantCount, opt => opt.MapFrom(src => src.Variants.Count));

        CreateMap<Variant, ProductVariantDto>()
            .ForMember(dest => dest.PriceOverride, opt => opt.MapFrom(src => src.PriceOverride != null ? MoneyDto.FromMoney(src.PriceOverride) : null));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.Slug.Value))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.BasePrice)))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.OrderBy(x => x.Sku)));
    }
}
