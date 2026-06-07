using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using System.Security.Claims;

namespace E_Commerce.API.Identity
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _ctx;

        public UserAccessor(IHttpContextAccessor ctx) => _ctx = ctx;

        private ClaimsPrincipal? User => _ctx.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                foreach (var claim in User?.Claims ?? [])
                {
                    Console.WriteLine($"{claim.Type} = {claim.Value}");
                }
                var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(id, out var g) ? g : null;
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

        public Guid GetRequiredUserId()
        {
            return UserId ?? throw new AppException(AuthErrors.InvalidToken);
        }

        public UserRole GetRequiredRole()
        {
            return Role ?? throw new AppException(AuthErrors.InvalidToken);
        }

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    }
}
