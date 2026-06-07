using E_Commerce.API.Common.Errors;
using E_Commerce.API.Common.Responses;
using E_Commerce.Domain.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace E_Commerce.API.Configuration;

public static class RateLimitingConfiguration
{
    public const string AuthLimiter = "AuthLimiter";
    public const string PublicLimiter = "PublicLimiter";
    public const string SearchLimiter = "SearchLimiter";
    public const string ExpensiveLimiter = "ExpensiveLimiter";

    public static IServiceCollection ApplyRateLimitingConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";
                var err = RateLimitApiErrors.TooManyRequests;
                var apiResponse = ApiResult.Fail(426,err.Code, err.Message);
                await context.HttpContext.Response.WriteAsJsonAsync(
                    apiResponse.Response,
                    new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });
            };

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var partitionKey = GetUserOrIpKey(context);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

            options.AddPolicy(AuthLimiter, context =>
            {
                var partitionKey = GetIpKey(context);

                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey,
                    _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(10),
                        SegmentsPerWindow = 5,
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });

            options.AddPolicy(SearchLimiter, context =>
            {
                var partitionKey = GetUserOrIpKey(context);

                return RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 50,
                        TokensPerPeriod = 10,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });

            options.AddPolicy(ExpensiveLimiter, context =>
            {
                var partitionKey = GetUserOrIpKey(context);

                return RateLimitPartition.GetConcurrencyLimiter(
                    partitionKey,
                    _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = 3,
                        QueueLimit = 0
                    });
            });
        });

        return services;
    }

    private static string GetUserOrIpKey(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrWhiteSpace(userId))
            return $"user:{userId}";

        return GetIpKey(context);
    }

    private static string GetIpKey(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();

        return !string.IsNullOrWhiteSpace(ip)
            ? $"ip:{ip}"
            : "ip:unknown";
    }
}