using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Application.Features.Auth.Commands.ChangeUserPassword;

public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;
    private readonly IPasswordHasherAdapter _passwordHasher;
    private readonly ILogger<ChangeUserPasswordHandler> _logger;

    public ChangeUserPasswordHandler(
        IUnitOfWork uow,
        IUserAccessor userAccessor,
        IPasswordHasherAdapter passwordHasher,
        ILogger<ChangeUserPasswordHandler> logger)
    {
        _uow = uow;
        _userAccessor = userAccessor;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
            return Result.Fail(AuthErrors.InvalidToken);

        User? user = await _uow.Users.GetByIdWithLoadingDataAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result.Fail(AuthErrors.InvalidToken);

        if (!_passwordHasher.Verify(user.Credential.PasswordHash, request.CurrentPassword))
            return Result.Fail(AuthErrors.InvalidCredentials);

        if (_passwordHasher.Verify(user.Credential.PasswordHash, request.NewPassword))
            return Result.Fail(AuthErrors.PasswordInvalid);

        PasswordHash newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.Credential.SetPassword(newPasswordHash);
        user.IsPasswordUpdated = true;

        await RevokeActiveRefreshTokensAsync(user.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Password changed for User {UserId}",
            user.Id);

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
