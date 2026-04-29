using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Common.Responses;

internal static class ApiResultMapper
{
    public static ApiResult FromResult(this ControllerBase controller, Result result, string successMessage, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return ApiResult.Success(successMessage, statusCode: successStatusCode);
        }

        var statusCode = MapStatusCode(result.Error);
        return ApiResult.Fail(statusCode, result.Error.Code, result.Error.Message);
    }

    public static ApiResult<T> FromResult<T>(this ControllerBase controller, Result<T> result, string successMessage, int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return ApiResult<T>.Success(result.Data!, successMessage, statusCode: successStatusCode);
        }

        var statusCode = MapStatusCode(result.Error);
        return ApiResult<T>.Fail(statusCode, result.Error.Code, result.Error.Message);
    }

    public static int MapStatusCode(Error error)
    {
        if (error.Code is ErrorCodes.Infrastructure.DatabaseUnavailable
            or ErrorCodes.External.ServiceUnavailable
            or ErrorCodes.External.PaymentProviderUnavailable
            or ErrorCodes.External.EmailProviderUnavailable)
        {
            return StatusCodes.Status503ServiceUnavailable;
        }

        return error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.External => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}