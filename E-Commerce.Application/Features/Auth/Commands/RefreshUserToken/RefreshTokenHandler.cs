using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.RefreshUserToken
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokensResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenHasher _tokenHasher;
        private readonly IGenerateLoginTokens _generateLoginTokens;

        public RefreshTokenHandler(IUnitOfWork uow, ITokenHasher tokenHasher, IGenerateLoginTokens generateLoginTokens)
        {
            _uow = uow;
            _tokenHasher = tokenHasher;
            _generateLoginTokens = generateLoginTokens;
        }

        public async Task<Result<RefreshTokensResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            TokenHash hashedToken = await _tokenHasher.HashAsync(request.RefreshToken, cancellationToken);
            RefreshToken? storedToken = await _uow.RefreshTokens.GetByHashedTokenAsync(hashedToken, cancellationToken);
            if (storedToken is null)
            {
                return Result<RefreshTokensResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidToken));
            }

            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (storedToken.IsExpired(now) || storedToken.IsRevoked)
            {
                return Result<RefreshTokensResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidToken));
            }

            User? existingUser = await _uow.Users.GetByIdAsync(storedToken.UserId, cancellationToken);
            if (existingUser is null)
            {
                return Result<RefreshTokensResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidToken));
            }

            (string accessToken, RefreshToken newStoredRefreshToken, string rawRefreshToken) = await _generateLoginTokens.GenerateTokensAsync(existingUser, cancellationToken);
            await _uow.RefreshTokens.CreateAsync(newStoredRefreshToken, cancellationToken);
            storedToken.Replace(newStoredRefreshToken.TokenHash, now);
            storedToken.Revoke(now);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<RefreshTokensResponse>.Success(new RefreshTokensResponse(accessToken, rawRefreshToken));
        }
    }
}

