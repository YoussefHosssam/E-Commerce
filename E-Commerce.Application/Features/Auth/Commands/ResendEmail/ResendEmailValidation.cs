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
                .NotEmpty().WithError(ErrorCodes.User.EmailRequired)
                .EmailAddress().WithError(ErrorCodes.User.EmailInvalid)
                .MaximumLength(256).WithError(ErrorCodes.User.EmailTooLong);
        }
    }
}
