using AutoMapper;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryListItemDto>()
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.ChildrenCount, opt => opt.MapFrom(src => src.Children.Count))
            .ForMember(dest => dest.ProductsCount, opt => opt.MapFrom(src => src.Products.Count));

        CreateMap<Category, CategoryChildDto>()
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value));

        CreateMap<Category, CategoryDetailDto>()
            .ForMember(dest => dest.ParentSlug, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Slug.Value : null))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug.Value))
            .ForMember(dest => dest.ProductsCount, opt => opt.MapFrom(src => src.Products.Count))
            .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children.OrderBy(x => x.SortOrder).ThenBy(x => x.Slug.Value)));
    }
}
