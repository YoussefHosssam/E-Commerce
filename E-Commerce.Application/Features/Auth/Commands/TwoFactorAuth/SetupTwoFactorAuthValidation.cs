using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth
{
    public class SetupTwoFactorAuthValidation : AbstractValidator<SetupTwoFactorAuthCommand>
    {
        public SetupTwoFactorAuthValidation()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithError(ErrorCodes.Auth.PasswordRequired)
                .MinimumLength(8).WithError(ErrorCodes.Auth.PasswordTooShort)
                .MaximumLength(128).WithError(ErrorCodes.Auth.PasswordTooLong);
        }
    }
}
