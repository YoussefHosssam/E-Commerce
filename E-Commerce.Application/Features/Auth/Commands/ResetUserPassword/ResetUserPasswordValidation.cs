using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.ResetUserPassword;

public sealed class ResetUserPasswordValidation : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordValidation()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Token)
            .NotEmpty().WithError(AuthErrors.TokenRequired);

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithError(AuthErrors.PasswordRequired)
            .MinimumLength(8).WithError(AuthErrors.PasswordTooShort)
            .MaximumLength(128).WithError(AuthErrors.PasswordTooLong)
            .Matches("[A-Z]").WithError(AuthErrors.PasswordUppercaseMissing)
            .Matches("[a-z]").WithError(AuthErrors.PasswordLowercaseMissing)
            .Matches("[0-9]").WithError(AuthErrors.PasswordDigitMissing)
            .Matches(@"[\W_]").WithError(AuthErrors.PasswordSpecialCharacterMissing)
            .Must(password => password == null || !password.Any(char.IsWhiteSpace)).WithError(AuthErrors.PasswordWhitespaceInvalid);
    }
}
