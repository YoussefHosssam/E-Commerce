using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens;
using E_Commerce.Application.Services.Contracts;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class GenerateLoginTokens : IGenerateLoginTokens
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public GenerateLoginTokens(IUnitOfWork uow, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _uow = uow;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokensAndSaveAsync(User user, CancellationToken ctn)
        {
            (string accessToken, RefreshToken newStoredRefreshToken, string rawRefreshToken) = await GenerateTokensAsync(user, ctn);
            await _uow.RefreshTokens.CreateAsync(newStoredRefreshToken, ctn);
            await _uow.SaveChangesAsync(ctn);
            return (accessToken, rawRefreshToken);
        }

        public async Task<(string accessToken, RefreshToken refreshToken, string rawRefreshToken)> GenerateTokensAsync(User user, CancellationToken ctn)
        {
            JwtTokenData accessTokenData = new(user.Id, user.Email, user.Role);
            string accessToken = _jwtTokenService.CreateAccessToken(accessTokenData);
            string rawRefreshToken; TokenHash HashedRefreshToken; DateTimeOffset expiresAt;
            (rawRefreshToken, HashedRefreshToken, expiresAt) = await _refreshTokenService.IssueAsync(user.Id, ctn);
            RefreshToken refreshToken = RefreshToken.Create(user.Id, HashedRefreshToken, expiresAt, DateTimeOffset.UtcNow);
            return (accessToken, refreshToken , rawRefreshToken);
        }
    }
}
