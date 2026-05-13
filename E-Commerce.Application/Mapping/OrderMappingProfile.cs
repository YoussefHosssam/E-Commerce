using AutoMapper;
using E_Commerce.Application.Features.Order.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderEntity = E_Commerce.Domain.Entities.Order;
using OrderItemEntity = E_Commerce.Domain.Entities.OrderItem;

namespace E_Commerce.Application.Mapping
{
    internal class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // 🔹 Order -> OrderListDto
            CreateMap<OrderEntity, OrderListDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Currency.Value));

            // 🔹 Order -> OrderDto
            CreateMap<OrderEntity, OrderDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Currency.Value))
                .ForMember(d => d.ShippingAddress, o => o.MapFrom(s => s.ShippingAddressJson.Value))
                .ForMember(d => d.BillingAddress, o => o.MapFrom(s => s.BillingAddressJson.Value))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            // 🔹 OrderItem -> DTO
            CreateMap<OrderItemEntity, OrderItemDto>()
                .ForMember(d => d.ProductTitle, o => o.MapFrom(s => s.ProductTitleSnapshot))
                .ForMember(d => d.Currency, o => o.MapFrom(s => s.Currency.Value));

        }
    }
}
