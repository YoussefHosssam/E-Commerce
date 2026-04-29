using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

internal static class JwtClaimsFactory
{
    public static IReadOnlyCollection<Claim> CreateClaims(Guid userId, EmailAddress email, UserRole role)
        => new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email.Value),
            new(ClaimTypes.Role, role.ToString()),
        }.AsReadOnly();
}