using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public class VerifyLoginTwoFactorAuthHandler : IRequestHandler<VerifyLoginTwoFactorAuthCommand, Result<FinalizeLoginResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IGenerateLoginTokens _generateLoginTokens;
        private readonly ITotpHandler _totpHandler;

        public VerifyLoginTwoFactorAuthHandler(IUnitOfWork uow, IGenerateLoginTokens generateLoginTokens, ITotpHandler totpHandler)
        {
            _uow = uow;
            _generateLoginTokens = generateLoginTokens;
            _totpHandler = totpHandler;
        }

        public async Task<Result<FinalizeLoginResponse>> Handle(VerifyLoginTwoFactorAuthCommand request, CancellationToken cancellationToken)
        {
            TwoFactorLoginChallenge? loginChallenge = await _uow.TwoFactorLoginChallenges.GetSingleByPredicateAsync(c => c.ChallengeId == request.ChallengeId, cancellationToken);
            if (loginChallenge is null || loginChallenge.IsExpired(DateTimeOffset.UtcNow) || loginChallenge.IsVerified)
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            User? existingUser = await _uow.Users.GetByIdWithLoadingDataAsync(loginChallenge.UserId, cancellationToken);
            if (existingUser?.TwoFactorAuth is null || string.IsNullOrWhiteSpace(existingUser.TwoFactorAuth.TotpSecretEncrypted))
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            if (!_totpHandler.VerifyCode(existingUser.TwoFactorAuth.TotpSecretEncrypted, request.OtpCode))
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            loginChallenge.MarkVerified(DateTimeOffset.UtcNow);
            await _uow.SaveChangesAsync(cancellationToken);

            (string accessToken, string refreshToken) = await _generateLoginTokens.GenerateTokensAndSaveAsync(existingUser, cancellationToken);
            var response = new FinalizeLoginResponse(accessToken, refreshToken, DateTimeOffset.UtcNow, false, false);
            return Result<FinalizeLoginResponse>.Success(response);
        }
    }
}

