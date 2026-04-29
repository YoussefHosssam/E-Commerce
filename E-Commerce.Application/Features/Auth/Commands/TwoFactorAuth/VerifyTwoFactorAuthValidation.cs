using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth
{
    public class VerifyTwoFactorAuthValidation : AbstractValidator<VerifyTwoFactorAuthCommand>
    {
        public VerifyTwoFactorAuthValidation()
        {
            RuleFor(x => x.OtpCode)
                .NotEmpty().WithError(ErrorCodes.Auth.InvalidRequest)
                .Matches(@"^\d{6}$").WithError(ErrorCodes.Auth.InvalidRequest);
        }
    }
}
