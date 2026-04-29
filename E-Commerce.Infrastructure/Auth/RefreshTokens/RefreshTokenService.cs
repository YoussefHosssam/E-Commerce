using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Auth.RefreshTokens
{
    internal class RefreshTokenService : IRefreshTokenService
    {
        private readonly RefreshTokenOptions _opt;
        private readonly ITokenHasher _hasher;
        public RefreshTokenService(IOptions<RefreshTokenOptions> opt, ITokenHasher hasher)
        {
            _opt = opt.Value;
            _hasher = hasher;
        }
        public async Task<(string ,TokenHash , DateTimeOffset)> IssueAsync(Guid userId, CancellationToken ct)
        {
            var raw = GenerateSecureToken(_opt.TokenBytes);
            var hash = await _hasher.HashAsync(raw , ct);

            var expires = DateTimeOffset.UtcNow.AddDays(_opt.DaysToExpire);

            return (raw ,hash , expires);
        }

        private static string GenerateSecureToken(int bytes)
        {
            var buff = RandomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(buff);
        }
    }
}
