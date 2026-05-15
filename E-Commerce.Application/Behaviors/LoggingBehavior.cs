using E_Commerce.Application.Common.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace E_Commerce.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long SlowRequestThresholdMs = 500;

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(
            "Handling application request {RequestName}",
            requestName);

        var response = await next();

        stopwatch.Stop();

        LogResult(requestName, stopwatch.ElapsedMilliseconds, response);

        return response;
    }

    private void LogResult(string requestName, long elapsedMs, TResponse response)
    {
        if (response is Result { IsSuccess: false } result)
        {
            _logger.LogWarning(
                "Application request {RequestName} completed with {ErrorCode} in {ElapsedMs} ms",
                requestName,
                result.Error?.Code,
                elapsedMs);

            return;
        }

        if (elapsedMs >= SlowRequestThresholdMs)
        {
            _logger.LogWarning(
                "Slow application request {RequestName} completed in {ElapsedMs} ms",
                requestName,
                elapsedMs);

            return;
        }

        _logger.LogDebug(
            "Application request {RequestName} completed in {ElapsedMs} ms",
            requestName,
            elapsedMs);
    }
}
