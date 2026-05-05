using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Checkout.Commands;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

public sealed class PlaceOrderValidation : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderValidation()
    {
        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithError(OrderErrors.ShippingAddress.Required);

        When(x => x.ShippingAddress is not null, () =>
        {
            RuleFor(x => x.ShippingAddress!.FullName)
                .NotEmpty()
                .MaximumLength(120)
                .WithError(OrderErrors.ShippingAddress.FullNameRequired);

            RuleFor(x => x.ShippingAddress!.Phone)
                .NotEmpty()
                .MaximumLength(30)
                .WithError(OrderErrors.ShippingAddress.PhoneRequired);

            RuleFor(x => x.ShippingAddress!.City)
                .NotEmpty()
                .MaximumLength(100)
                .WithError(OrderErrors.ShippingAddress.CityRequired);

            RuleFor(x => x.ShippingAddress!.AddressLine1)
                .NotEmpty()
                .MaximumLength(250)
                .WithError(OrderErrors.ShippingAddress.AddressLine1Required);

            RuleFor(x => x.ShippingAddress!.AddressLine2)
                .MaximumLength(250)
                .When(x => x.ShippingAddress!.AddressLine2 is not null)
                .WithError(OrderErrors.ShippingAddress.AddressLine1Required);
        });

        When(x => !x.SameAsShipping, () =>
        {
            RuleFor(x => x.BillingAddress)
                .NotNull()
                .WithError(OrderErrors.BillingAddress.Required);

            When(x => x.BillingAddress is not null, () =>
            {
                RuleFor(x => x.BillingAddress!.FullName)
                    .NotEmpty()
                    .MaximumLength(120)
                    .WithError(OrderErrors.BillingAddress.FullNameRequired);

                RuleFor(x => x.BillingAddress!.Phone)
                    .NotEmpty()
                    .MaximumLength(30)
                    .WithError(OrderErrors.BillingAddress.PhoneRequired);

                RuleFor(x => x.BillingAddress!.City)
                    .NotEmpty()
                    .MaximumLength(100)
                    .WithError(OrderErrors.BillingAddress.CityRequired);

                RuleFor(x => x.BillingAddress!.AddressLine1)
                    .NotEmpty()
                    .MaximumLength(250)
                    .WithError(OrderErrors.BillingAddress.AddressLine1Required);

                RuleFor(x => x.BillingAddress!.AddressLine2)
                    .MaximumLength(250)
                    .When(x => x.BillingAddress!.AddressLine2 is not null)
                    .WithError(OrderErrors.BillingAddress.AddressLine1Required);
            });
        });
    }
}