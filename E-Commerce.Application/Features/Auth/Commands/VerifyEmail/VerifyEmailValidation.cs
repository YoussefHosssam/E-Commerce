using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailValidation : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailValidation()
        {
            RuleFor(v => v.token)
                .NotEmpty()
                .WithError(ErrorCodes.Auth.TokenRequired);
        }
    }
}
