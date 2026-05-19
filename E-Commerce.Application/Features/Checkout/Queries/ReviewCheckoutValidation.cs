using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Checkout.Queries;

public sealed class ReviewCheckoutValidation : AbstractValidator<ReviewCheckoutQuery>
{
    public ReviewCheckoutValidation()
    {
        RuleFor(x => x.AddressId)
            .NotNull()
            .WithError(CheckoutErrors.AddressRequired)
            .When(x => !x.DefaultAddress);

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
