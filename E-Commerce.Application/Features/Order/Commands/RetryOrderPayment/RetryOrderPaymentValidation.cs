using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Order.Commands.RetryOrderPayment;

public sealed class RetryOrderPaymentValidation : AbstractValidator<RetryOrderPaymentCommand>
{
    public RetryOrderPaymentValidation()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithError(OrderErrors.IdRequired);
    }
}
