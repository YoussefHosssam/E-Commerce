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
                .NotEmpty().WithError(AuthErrors.EmailRequired)
                .EmailAddress().WithError(AuthErrors.EmailInvalid)
                .MaximumLength(256).WithError(AuthErrors.EmailTooLong);

            RuleFor(x => x.Password)
                .NotEmpty().WithError(AuthErrors.PasswordRequired)
                .MinimumLength(8).WithError(AuthErrors.PasswordTooShort)
                .MaximumLength(128).WithError(AuthErrors.PasswordTooLong);
        }
    }
}
