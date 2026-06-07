using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.API.Common.Errors;

public static class RateLimitApiErrors
{
    public static readonly Error TooManyRequests =
        new("RATE_LIMIT_429_TOO_MANY_REQUESTS",
            "Too many requests. Please try again later.",
            ErrorType.Validation);

    public static readonly Error ConcurrentRequestLimitExceeded =
        new("RATE_LIMIT_429_CONCURRENT_REQUEST_LIMIT_EXCEEDED",
            "Too many concurrent requests. Please try again later.",
            ErrorType.Validation);
}