using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Cart.Commands.AddItem
{
    internal sealed class AddItemToCartValidation : AbstractValidator<AddItemToCartCommand>
    {
        public AddItemToCartValidation()
        {
            RuleFor(x => x.variantId)
                .NotEmpty()
                .WithError(CartItemErrors.VariantIdRequired);

            RuleFor(x => x.quantity)
                .GreaterThan(0)
                .WithError(CartItemErrors.QuantityInvalid);
        }
    }
}