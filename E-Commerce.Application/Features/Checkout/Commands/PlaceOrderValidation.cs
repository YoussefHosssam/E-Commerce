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
            RuleFor(x => x.ShippingAddress!.FirstName)
                .NotEmpty()
                .MaximumLength(60)
                .WithError(OrderErrors.ShippingAddress.FirstNameRequired);

            RuleFor(x => x.ShippingAddress!.LastName)
                .NotEmpty()
                .MaximumLength(60)
                .WithError(OrderErrors.ShippingAddress.LastNameRequired);

            RuleFor(x => x.ShippingAddress!.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150)
                .WithError(OrderErrors.ShippingAddress.EmailRequired);

            RuleFor(x => x.ShippingAddress!.PhoneNumber)
                .NotEmpty()
                .MaximumLength(30)
                .WithError(OrderErrors.ShippingAddress.PhoneRequired);

            RuleFor(x => x.ShippingAddress!.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.ShippingAddress!.City))
                .WithError(OrderErrors.ShippingAddress.CityTooLong);

            RuleFor(x => x.ShippingAddress!.AddressLine1)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.ShippingAddress!.AddressLine1))
                .WithError(OrderErrors.ShippingAddress.AddressLineTooLong);

            RuleFor(x => x.ShippingAddress!.AddressLine2)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.ShippingAddress!.AddressLine2))
                .WithError(OrderErrors.ShippingAddress.AddressLineTooLong);
        });

        When(x => !x.SameAsShipping, () =>
        {
            RuleFor(x => x.BillingAddress)
                .NotNull()
                .WithError(OrderErrors.BillingAddress.Required);

            When(x => x.BillingAddress is not null, () =>
            {
                RuleFor(x => x.BillingAddress!.FirstName)
                    .NotEmpty()
                    .MaximumLength(60)
                    .WithError(OrderErrors.BillingAddress.FirstNameRequired);

                RuleFor(x => x.BillingAddress!.LastName)
                    .NotEmpty()
                    .MaximumLength(60)
                    .WithError(OrderErrors.BillingAddress.LastNameRequired);

                RuleFor(x => x.BillingAddress!.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .MaximumLength(150)
                    .WithError(OrderErrors.BillingAddress.EmailRequired);

                RuleFor(x => x.BillingAddress!.PhoneNumber)
                    .NotEmpty()
                    .MaximumLength(30)
                    .WithError(OrderErrors.BillingAddress.PhoneRequired);

                RuleFor(x => x.BillingAddress!.City)
                    .MaximumLength(100)
                    .When(x => !string.IsNullOrWhiteSpace(x.BillingAddress!.City))
                    .WithError(OrderErrors.BillingAddress.CityTooLong);

                RuleFor(x => x.BillingAddress!.AddressLine1)
                    .MaximumLength(250)
                    .When(x => !string.IsNullOrWhiteSpace(x.BillingAddress!.AddressLine1))
                    .WithError(OrderErrors.BillingAddress.AddressLineTooLong);

                RuleFor(x => x.BillingAddress!.AddressLine2)
                    .MaximumLength(250)
                    .When(x => !string.IsNullOrWhiteSpace(x.BillingAddress!.AddressLine2))
                    .WithError(OrderErrors.BillingAddress.AddressLineTooLong);
            });
        });
    }
}