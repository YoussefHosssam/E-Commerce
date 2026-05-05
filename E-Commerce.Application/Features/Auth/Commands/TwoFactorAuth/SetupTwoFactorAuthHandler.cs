using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth
{
    public class SetupTwoFactorAuthHandler : IRequestHandler<SetupTwoFactorAuthCommand, Result<SetupTwoFactorAuthResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasherAdapter _passwordHasherAdapter;
        private readonly IUserAccessor _userAccessor;
        private readonly ITotpHandler _totpHandler;

        public SetupTwoFactorAuthHandler(IUnitOfWork uow, IPasswordHasherAdapter passwordHasher, IUserAccessor userAccessor, ITotpHandler totpHandler)
        {
            _uow = uow;
            _passwordHasherAdapter = passwordHasher;
            _userAccessor = userAccessor;
            _totpHandler = totpHandler;
        }

        public async Task<Result<SetupTwoFactorAuthResponse>> Handle(SetupTwoFactorAuthCommand request, CancellationToken cancellationToken)
        {
            if (!_userAccessor.UserId.HasValue)
            {
                return Result<SetupTwoFactorAuthResponse>.Fail(AuthErrors.InvalidCredentials);
            }

            User? existingUser = await _uow.Users.GetByIdWithLoadingDataAsync(_userAccessor.UserId.Value, cancellationToken);
            if (existingUser is null)
            {
                return Result<SetupTwoFactorAuthResponse>.Fail(AuthErrors.InvalidCredentials);
            }

            bool isCorrectPassword = _passwordHasherAdapter.Verify(existingUser.Credential.PasswordHash, request.Password);
            if (!isCorrectPassword)
            {
                return Result<SetupTwoFactorAuthResponse>.Fail(AuthErrors.PasswordInvalid);
            }

            (string manualKey, string encryptedSecretKey) = _totpHandler.GenerateSecretKey();
            string qrCodeUrl = await _totpHandler.GenerateQrCodeAsync(manualKey, existingUser.Email, cancellationToken);
            existingUser.SetupTwoFactor(encryptedSecretKey, DateTimeOffset.UtcNow);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<SetupTwoFactorAuthResponse>.Success(new SetupTwoFactorAuthResponse(qrCodeUrl, manualKey));
        }
    }
}

