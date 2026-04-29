using E_Commerce.API.Common.Responses;
using E_Commerce.Domain.Common.Errors;
using MailKit.Net.Smtp;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                var error = MapException(exception);
                _logger.LogError(exception, "Unhandled exception translated to error code {ErrorCode}", error.Code);

                var statusCode = ApiResultMapper.MapStatusCode(error);
                var apiResponse = ApiResult.Fail(statusCode, error.Code, error.Message);

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(apiResponse.Response);
            }
        }

        private static Error MapException(Exception exception)
        {
            return exception switch
            {
                AppException appException => appException.Error,
                DbUpdateException => InfrastructureErrors.DatabaseUnavailable,
                SqlException => InfrastructureErrors.DatabaseUnavailable,
                TimeoutException => ExternalErrors.ServiceUnavailable,
                HttpRequestException => ExternalErrors.ServiceUnavailable,
                SmtpCommandException => ExternalErrors.EmailProviderUnavailable,
                SmtpProtocolException => ExternalErrors.EmailProviderUnavailable,
                _ => CommonErrors.Unexpected("An unexpected error occurred.")
            };
        }
    }
}
