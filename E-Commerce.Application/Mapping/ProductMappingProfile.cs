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
            .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => src.CompareAtPrice != null ? MoneyDto.FromMoney(src.CompareAtPrice) : null))
            .ForMember(dest => dest.DefaultVariantId, opt => opt.MapFrom(src => src.Variants.FirstOrDefault(v => v.IsActive && v.IsDefault) != null ? src.Variants.First(v => v.IsActive && v.IsDefault).Id : (Guid?)null))
            .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => src.Variants.Any(v => v.IsActive) ? MoneyDto.FromMoney(src.Variants.Where(v => v.IsActive).OrderBy(v => v.GetPrice().Amount).First().GetPrice()) : null))
            .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => src.Variants.Any(v => v.IsActive) ? MoneyDto.FromMoney(src.Variants.Where(v => v.IsActive).OrderByDescending(v => v.GetPrice().Amount).First().GetPrice()) : null))
            .ForMember(dest => dest.VariantCount, opt => opt.MapFrom(src => src.Variants.Count));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.Slug.Value))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => MoneyDto.FromMoney(src.BasePrice)))
            .ForMember(dest => dest.CompareAtPrice, opt => opt.MapFrom(src => src.CompareAtPrice != null ? MoneyDto.FromMoney(src.CompareAtPrice) : null))
            .ForMember(dest => dest.DefaultVariantId, opt => opt.MapFrom(src => src.Variants.FirstOrDefault(v => v.IsActive && v.IsDefault) != null ? src.Variants.First(v => v.IsActive && v.IsDefault).Id : (Guid?)null))
            .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => src.Variants.Any(v => v.IsActive) ? MoneyDto.FromMoney(src.Variants.Where(v => v.IsActive).OrderBy(v => v.GetPrice().Amount).First().GetPrice()) : null))
            .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => src.Variants.Any(v => v.IsActive) ? MoneyDto.FromMoney(src.Variants.Where(v => v.IsActive).OrderByDescending(v => v.GetPrice().Amount).First().GetPrice()) : null))
            .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants.Where(x => x.IsActive).OrderBy(x => x.Sku)));
    }
}
