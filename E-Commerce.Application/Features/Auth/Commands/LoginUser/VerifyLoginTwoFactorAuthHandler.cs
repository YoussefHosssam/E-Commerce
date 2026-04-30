using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
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
        private readonly ICartMergeService _cartMergeService;

        public VerifyLoginTwoFactorAuthHandler(IUnitOfWork uow, IGenerateLoginTokens generateLoginTokens, ITotpHandler totpHandler, ICartMergeService cartMergeService)
        {
            _uow = uow;
            _generateLoginTokens = generateLoginTokens;
            _totpHandler = totpHandler;
            _cartMergeService = cartMergeService;
        }

        public async Task<Result<FinalizeLoginResponse>> Handle(VerifyLoginTwoFactorAuthCommand request, CancellationToken cancellationToken)
        {
            TwoFactorLoginChallenge? loginChallenge = await _uow.TwoFactorLoginChallenges.GetSingleByPredicateAsync(c => c.ChallengeId == request.ChallengeId, cancellationToken);
            if (loginChallenge is null || loginChallenge.IsExpired(DateTimeOffset.UtcNow) || loginChallenge.IsVerified)
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            User? existUser = await _uow.Users.GetByIdWithLoadingDataAsync(loginChallenge.UserId, cancellationToken);
            if (existUser?.TwoFactorAuth is null || string.IsNullOrWhiteSpace(existUser.TwoFactorAuth.TotpSecretEncrypted))
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            if (!_totpHandler.VerifyCode(existUser.TwoFactorAuth.TotpSecretEncrypted, request.OtpCode))
            {
                return Result<FinalizeLoginResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            loginChallenge.MarkVerified(DateTimeOffset.UtcNow);
            await _uow.SaveChangesAsync(cancellationToken);

            (string accessToken, string refreshToken) = await _generateLoginTokens.GenerateTokensAndSaveAsync(existUser, cancellationToken);
            var response = new FinalizeLoginResponse(accessToken, refreshToken, DateTimeOffset.UtcNow, false, false);
            (bool isNeedToMerge, string anonymousToken) = _cartMergeService.IsNeedToBeMerged();
            if (isNeedToMerge) await _cartMergeService.MergeCarts(existUser.Id, anonymousToken, cancellationToken);
            return Result<FinalizeLoginResponse>.Success(response);
        }
    }
}

