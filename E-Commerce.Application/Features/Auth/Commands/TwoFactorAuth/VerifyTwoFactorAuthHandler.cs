using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth
{
    public class VerifyTwoFactorAuthHandler : IRequestHandler<VerifyTwoFactorAuthCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITotpHandler _totpHandler;
        private readonly IUserAccessor _userAccessor;

        public VerifyTwoFactorAuthHandler(IUnitOfWork uow, ITotpHandler totpHandler, IUserAccessor userAccessor)
        {
            _uow = uow;
            _totpHandler = totpHandler;
            _userAccessor = userAccessor;
        }

        public async Task<Result> Handle(VerifyTwoFactorAuthCommand request, CancellationToken cancellationToken)
        {
            if (!_userAccessor.UserId.HasValue)
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidCredentials));
            }

            User? existingUser = await _uow.Users.GetByIdWithLoadingDataAsync(_userAccessor.UserId.Value, cancellationToken);
            if (existingUser is null)
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidCredentials));
            }

            if (existingUser.TwoFactorAuth is null)
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorSetupRequired));
            }

            var encryptedSecretKey = existingUser.TwoFactorAuth.TotpSecretEncrypted;
            if (string.IsNullOrWhiteSpace(encryptedSecretKey) || !_totpHandler.VerifyCode(encryptedSecretKey, request.OtpCode))
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.TwoFactorInvalid));
            }

            existingUser.EnableTwoFactor(DateTimeOffset.UtcNow);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

