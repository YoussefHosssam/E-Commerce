using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem
{
    internal class RemoveItemValidation : AbstractValidator<RemoveItemCommand>
    {
        public RemoveItemValidation()
        {
            RuleFor(i => i.cartItemId)
                .NotEmpty()
                .WithError(CartItemErrors.CartItemIdRequired);
        }
    }
}
