using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserValidation : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidation()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Email)
                .NotEmpty().WithError(ErrorCodes.Auth.EmailRequired)
                .EmailAddress().WithError(ErrorCodes.Auth.EmailInvalid)
                .MaximumLength(256).WithError(ErrorCodes.Auth.EmailTooLong);

            RuleFor(x => x.Password)
                .NotEmpty().WithError(ErrorCodes.Auth.PasswordRequired)
                .MinimumLength(8).WithError(ErrorCodes.Auth.PasswordTooShort)
                .MaximumLength(128).WithError(ErrorCodes.Auth.PasswordTooLong);
        }
    }
}
