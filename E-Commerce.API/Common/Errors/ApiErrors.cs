using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.API.Common.Errors;

public static class ApiErrors
{
    public static readonly Error Unexpected =
        new(
            "API_500_UNEXPECTED",
            "An unexpected error occurred.",
            ErrorType.Failure);

    public static readonly Error RouteNotFound =
        new(
            "API_404_ROUTE_NOT_FOUND",
            "The requested endpoint was not found.",
            ErrorType.NotFound);

    public static readonly Error InvalidRouteParameter =
        new(
            "API_400_INVALID_ROUTE_PARAMETER",
            "One or more route parameters are invalid.",
            ErrorType.Validation);

    public static readonly Error InvalidRequest =
        new(
            "API_400_INVALID_REQUEST",
            "The request is invalid.",
            ErrorType.Validation);

    public static readonly Error InvalidJson =
        new(
            "API_400_INVALID_JSON",
            "The request body contains invalid JSON.",
            ErrorType.Validation);

    public static readonly Error UnsupportedMediaType =
        new(
            "API_415_UNSUPPORTED_MEDIA_TYPE",
            "The request content type is not supported.",
            ErrorType.Validation);

    public static readonly Error Unauthorized =
        new(
            "API_401_UNAUTHORIZED",
            "Authentication is required.",
            ErrorType.Unauthorized);

    public static readonly Error Forbidden =
        new(
            "API_403_FORBIDDEN",
            "You do not have permission to perform this action.",
            ErrorType.Forbidden);

    public static readonly Error TooManyRequests =
        new(
            "API_429_TOO_MANY_REQUESTS",
            "Too many requests. Please try again later.",
            ErrorType.Failure);
}