using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem;

internal sealed class RemoveItemValidation : AbstractValidator<RemoveItemCommand>
{
    public RemoveItemValidation()
    {
        RuleFor(x => x.variantId)
            .NotEmpty()
            .WithError(ErrorCodes.CartItem.VariantIdRequired);

        RuleFor(x => x.quantity)
            .GreaterThan(0)
            .WithError(ErrorCodes.CartItem.QuantityInvalid);
    }
}