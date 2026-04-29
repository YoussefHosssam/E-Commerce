using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _ctx;

    public UserAccessor(IHttpContextAccessor ctx) => _ctx = ctx;

    private ClaimsPrincipal? User => _ctx.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(id, out var g) ? g : null;
        }
    }

    public EmailAddress? Email
    {
        get
        {
            var email = User?.FindFirst(ClaimTypes.Email)?.Value;
            return string.IsNullOrEmpty(email) ? null : EmailAddress.Create(email);
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse(role, true, out UserRole result) ? result : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

}