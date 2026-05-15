using Serilog.Context;
using System.Security.Claims;

namespace E_Commerce.API.Middlewares;

public sealed class SerilogContextEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public SerilogContextEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        var clientIp = context.Connection.RemoteIpAddress?.ToString();

        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserRole", userRole))
        using (LogContext.PushProperty("IsAuthenticated", context.User.Identity?.IsAuthenticated ?? false))
        using (LogContext.PushProperty("ClientIp", clientIp))
        using (LogContext.PushProperty("RequestPath", context.Request.Path.Value))
        {
            await _next(context);
        }
    }
}
