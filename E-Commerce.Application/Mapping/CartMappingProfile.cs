using AutoMapper;
using E_Commerce.Application.Features.Cart.Common;
using E_Commerce.Application.Features.Variant.Common;

namespace E_Commerce.Application.Mapping;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<Domain.Entities.Cart, CartSummaryDTO>()
            .ConstructUsing((src, ctx) => new CartSummaryDTO(
                src.Id,
                ctx.Mapper.Map<List<CartItemDTO>>(src.Items),
                src.GetTotalQuantity(),
                src.GetTotalPrice(),
                src.AnonymousToken));

        CreateMap<Domain.Entities.CartItem, CartItemDTO>()
            .ConstructUsing((src, ctx) => new CartItemDTO(
                src.Id,
                src.Quantity,
                ctx.Mapper.Map<CartVariantDto>(src.Variant)));
    }
}
