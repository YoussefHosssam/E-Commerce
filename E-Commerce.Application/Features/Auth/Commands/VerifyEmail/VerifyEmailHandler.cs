using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;

namespace E_Commerce.Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenHasher _tokenHasher;

        public VerifyEmailHandler(IUnitOfWork uow, ITokenHasher tokenHasher)
        {
            _uow = uow;
            _tokenHasher = tokenHasher;
        }

        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            TokenHash hashedRequestToken = await _tokenHasher.HashAsync(request.token, cancellationToken);
            AuthToken? savedRequestToken = await _uow.AuthTokens.GetSingleByPredicateAsync(at => at.TokenHash == hashedRequestToken && at.TokenType == TokenType.VerifyEmailToken, cancellationToken);
            if (savedRequestToken is null || savedRequestToken.IsTokenConsumed())
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidToken));
            }

            if (savedRequestToken.IsTokenExpired(DateTimeOffset.UtcNow))
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.ExpiredToken));
            }

            User? user = await _uow.Users.GetByIdAsync(savedRequestToken.UserId, cancellationToken);
            if (user is null)
            {
                return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidToken));
            }

            user.Verify();
            savedRequestToken.Consume();
            await _uow.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
