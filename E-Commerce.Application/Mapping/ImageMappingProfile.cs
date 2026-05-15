using AutoMapper;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Mapping
{
    public class ImageMappingProfile : Profile
    {
        public ImageMappingProfile()
        {
            CreateMap<VariantImage, ImageDto>().ForMember(i => i.ProcessingStatus, src => src.MapFrom(s => nameof(s.ProcessingStatus)));
            CreateMap<ProductImage, ImageDto>().ForMember(i => i.ProcessingStatus, src => src.MapFrom(s => nameof(s.ProcessingStatus)));
        }
    }
}