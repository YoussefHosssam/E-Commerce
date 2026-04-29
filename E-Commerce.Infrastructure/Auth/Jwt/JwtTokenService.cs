using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using E_Commerce.Infrastructure.Settings;

namespace E_Commerce.Infrastructure.Auth.Jwt
{
    internal class JwtTokenService : IJwtTokenService
    {

        private readonly JwtOptions _opt;

        public JwtTokenService(IOptions<JwtOptions> opt) => _opt = opt.Value;
        public string CreateAccessToken(JwtTokenData jwtTokenData)
        {
            var claims = JwtClaimsFactory.CreateClaims(jwtTokenData.UserId, jwtTokenData.Email, jwtTokenData.Role);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
