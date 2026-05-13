using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.ResetUserPassword;

internal class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenHasher _tokenHasher;
    private readonly IPasswordHasherAdapter _passwordHasher;

    public ResetUserPasswordHandler(
        IUnitOfWork uow,
        ITokenHasher tokenHasher,
        IPasswordHasherAdapter passwordHasher)
    {
        _uow = uow;
        _tokenHasher = tokenHasher;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        TokenHash hashedRequestToken = await _tokenHasher.HashAsync(request.Token, cancellationToken);

        AuthToken? savedRequestToken = await _uow.AuthTokens.GetSingleByPredicateAsync(
            at => at.TokenHash == hashedRequestToken &&
                  at.TokenType == TokenType.ResetPasswordToken,
            cancellationToken);

        if (savedRequestToken is null || savedRequestToken.IsTokenConsumed())
            return Result.Fail(AuthErrors.InvalidToken);

        if (savedRequestToken.IsTokenExpired(DateTimeOffset.UtcNow))
            return Result.Fail(AuthErrors.ExpiredToken);

        User? user = await _uow.Users.GetByIdWithLoadingDataAsync(savedRequestToken.UserId, cancellationToken);
        if (user is null)
            return Result.Fail(AuthErrors.InvalidToken);

        if (_passwordHasher.Verify(user.Credential.PasswordHash, request.NewPassword))
            return Result.Fail(AuthErrors.PasswordInvalid);

        PasswordHash newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.Credential.SetPassword(newPasswordHash);
        user.IsPasswordUpdated = true;
        savedRequestToken.Consume();

        await RevokeActiveRefreshTokensAsync(user.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task RevokeActiveRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var activeTokens = await _uow.RefreshTokens.GetActiveTokensByUserIdAsync(userId, now, cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke(now);
        }
    }
}
