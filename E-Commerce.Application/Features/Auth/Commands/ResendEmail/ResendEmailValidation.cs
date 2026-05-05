using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.VerifyEmail
{
    public class ResendEmailValidation : AbstractValidator<ResendEmailCommand>
    {
        public ResendEmailValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithError(UserErrors.EmailRequired)
                .EmailAddress().WithError(UserErrors.EmailInvalid)
                .MaximumLength(256).WithError(UserErrors.EmailTooLong);
        }
    }
}
