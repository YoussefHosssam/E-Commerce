using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Cart.Commands
{
    public record CartSummaryDTO (Guid cartId , List<CartItemDTO> items , int totalQuantity , decimal totalPrice , string? cartAnonymousToken)
    {
        public static CartSummaryDTO Empty()
        {
            return new CartSummaryDTO(Guid.Empty, new List<CartItemDTO>(), 0, 0, null);
        }
    }
    public record CartItemDTO (Guid cartItemId , int quantity , CartVariantDto variant);
    public static class CartMapping {
        public static List<CartItemDTO> ToCartItemListDTO(this IReadOnlyCollection<CartItem> items){
            List<CartItemDTO> cartItemDTOs = new();
            foreach(var item in items)
            {
                cartItemDTOs.Add(item.ToCartItemDTO());
            }
            return cartItemDTOs;
        }
        public static CartItemDTO ToCartItemDTO(this CartItem item)
        {
            return new CartItemDTO(item.Id, item.Quantity, item.Variant.ToCartVariantDto());
        }
        
    }
}
