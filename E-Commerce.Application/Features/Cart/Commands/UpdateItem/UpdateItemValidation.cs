using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem;

internal sealed class UpdateItemValidation : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemValidation()
    {
        RuleFor(x => x.cartItemId)
            .NotEmpty()
            .WithError(CartItemErrors.VariantIdRequired);

        RuleFor(x => x.quantity)
            .GreaterThan(0)
            .WithError(CartItemErrors.QuantityInvalid);
    }
}