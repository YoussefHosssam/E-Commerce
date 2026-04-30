using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Cart.Commands
{
    public record CartSummaryDTO (Guid cartId , List<CartItem> items , int totalQuantity , decimal totalPrice , string? cartAnonymousToken);
    
}
