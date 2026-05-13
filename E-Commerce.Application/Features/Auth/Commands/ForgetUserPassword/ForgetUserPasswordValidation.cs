using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.ForgetUserPassword;

public sealed class ForgetUserPasswordValidation : AbstractValidator<ForgetUserPasswordCommand>
{
    public ForgetUserPasswordValidation()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().WithError(AuthErrors.EmailRequired)
            .EmailAddress().WithError(AuthErrors.EmailInvalid)
            .MaximumLength(256).WithError(AuthErrors.EmailTooLong);
    }
}
