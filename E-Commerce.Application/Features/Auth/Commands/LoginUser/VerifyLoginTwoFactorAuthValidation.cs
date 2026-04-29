using E_Commerce.Application.Extensions;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public class VerifyLoginTwoFactorAuthValidation : AbstractValidator<VerifyLoginTwoFactorAuthCommand>
    {
        public VerifyLoginTwoFactorAuthValidation()
        {
            RuleFor(x => x.ChallengeId)
                .NotEmpty().WithError(ErrorCodes.Auth.InvalidRequest);

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithError(ErrorCodes.Auth.InvalidRequest)
                .Matches(@"^\d{6}$").WithError(ErrorCodes.Auth.InvalidRequest);
        }
    }
}
