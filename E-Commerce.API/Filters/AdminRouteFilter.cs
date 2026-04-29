using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_Commerce.API.Filters
{
    public class AdminRouteFilter : IAsyncAuthorizationFilter
    {
        private readonly IUserAccessor _userAccessor;

        public AdminRouteFilter(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!_userAccessor.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            if (_userAccessor.Role != Domain.Enums.UserRole.Admin)
            {
                context.Result = new ForbidResult();
            }

            return Task.CompletedTask;
        }
    }
}