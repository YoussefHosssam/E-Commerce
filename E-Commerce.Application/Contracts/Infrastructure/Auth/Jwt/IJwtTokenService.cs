using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt
{
    public interface IJwtTokenService
    {
        public string CreateAccessToken(JwtTokenData jwtTokenData);
    }
    public sealed record JwtTokenData(
    Guid UserId,
    EmailAddress Email,
    UserRole Role
);
}
