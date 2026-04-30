using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem
{
    public record RemoveItemCommand(Guid variantId , int quantity) : IRequest<Result<CartSummaryDTO>>;
}
