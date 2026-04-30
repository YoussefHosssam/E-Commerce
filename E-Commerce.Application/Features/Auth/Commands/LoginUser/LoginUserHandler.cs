using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasherAdapter _passwordHasherAdapter;
        private readonly IRandomStringGenerator _randomStringGenerator;
        private readonly IGenerateLoginTokens _generateLoginTokens;
        private readonly ICartMergeService _cartMergeService;


        public LoginUserHandler(IUnitOfWork uow, IPasswordHasherAdapter passwordHasherAdapter, IRandomStringGenerator randomStringGenerator, IGenerateLoginTokens generateAccessAndRefreshTokens, ICartMergeService cartMergeService)
        {
            _uow = uow;
            _passwordHasherAdapter = passwordHasherAdapter;
            _randomStringGenerator = randomStringGenerator;
            _generateLoginTokens = generateAccessAndRefreshTokens;
            _cartMergeService = cartMergeService;
        }

        public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            User? existUser = await _uow.Users.GetUserByEmailAsync(EmailAddress.Create(request.Email), cancellationToken);
            if (existUser == null) return Result<LoginUserResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.User.NotFound));
            PasswordHash existHashedPassword = existUser.Credential.PasswordHash;
            bool isCorrectPassword = _passwordHasherAdapter.Verify(existHashedPassword, request.Password);
            if (!isCorrectPassword) return Result<LoginUserResponse>.Fail(ErrorCatalog.FromCode(ErrorCodes.Auth.InvalidCredentials));
            if (existUser.IsTwoFactorAuthEnabled)
            {
                return await HandleLoginWithEnabled2fa(existUser, cancellationToken);
            }
            else
            {
                (bool isNeedToMerge, string anonymousToken) = _cartMergeService.IsNeedToBeMerged();
                if (isNeedToMerge) await _cartMergeService.MergeCarts(existUser.Id, anonymousToken, cancellationToken);
                return await HandleLoginWithDisabled2fa(existUser, cancellationToken);
            }
        }

        private async Task<Result<LoginUserResponse>> HandleLoginWithDisabled2fa (User user , CancellationToken ctn)
        {
            (string accessToken, string refreshToken) = await _generateLoginTokens.GenerateTokensAndSaveAsync(user, ctn);
            FinalizeLoginResponse responseObject = new FinalizeLoginResponse(accessToken, refreshToken, DateTimeOffset.UtcNow);

            return Result<LoginUserResponse>.Success(responseObject);
        }
        private async Task<Result<LoginUserResponse>> HandleLoginWithEnabled2fa(User user, CancellationToken ctn)
        {
            var now = DateTimeOffset.UtcNow;

            var challengeId = _randomStringGenerator.GetRandomString(); 
            var expiresAt = now.AddMinutes(5);

            var challenge = TwoFactorLoginChallenge.Create(
                user.Id,
                challengeId,
                expiresAt
            );

            await _uow.TwoFactorLoginChallenges.CreateAsync(challenge, ctn);
            await _uow.SaveChangesAsync(ctn);
            LoginUserResponseWith2faEnabled responseObject = new(challengeId);
            return Result<LoginUserResponse>.Success(responseObject);
        }
    }
}

