using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Cart.Common
{
    public record CartSummaryDTO (Guid cartId , List<CartItemDTO> items , int totalQuantity , decimal totalPrice , string? cartAnonymousToken)
    {
        public static CartSummaryDTO Empty()
        {
            return new CartSummaryDTO(Guid.Empty, new List<CartItemDTO>(), 0, 0, null);
        }
    }
    public record CartItemDTO (Guid cartItemId , int quantity , CartVariantDto variant);
}
