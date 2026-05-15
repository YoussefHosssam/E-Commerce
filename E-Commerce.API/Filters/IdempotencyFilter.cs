using E_Commerce.API.Attributes;
using E_Commerce.API.Common.Contracts;
using E_Commerce.API.Common.Errors;
using E_Commerce.API.Common.Responses;
using E_Commerce.Application.Contracts.API;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace E_Commerce.API.Filters;

public sealed class IdempotencyFilter : IAsyncResourceFilter
{
    private const string HeaderName = "X-Idempotency-Key";

    private readonly IIdempotencyStore _store;
    private readonly IUserAccessor _userAccessor;
    private readonly ILogger<IdempotencyFilter> _logger;

    public IdempotencyFilter(
        IIdempotencyStore store,
        IUserAccessor userAccessor,
        ILogger<IdempotencyFilter> logger)
    {
        _store = store;
        _userAccessor = userAccessor;
        _logger = logger;
    }

    public async Task OnResourceExecutionAsync(
        ResourceExecutingContext context,
        ResourceExecutionDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();

        var attribute = endpoint?.Metadata.GetMetadata<IdempotentAttribute>();

        if (attribute is null)
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;

        if (!HttpMethods.IsPost(request.Method) &&
            !HttpMethods.IsPut(request.Method) &&
            !HttpMethods.IsPatch(request.Method))
        {
            context.Result = Fail(IdempotencyApiErrors.UnsupportedMethod);
            return;
        }

        if (!request.Headers.TryGetValue(HeaderName, out var keyValues))
        {
            context.Result = Fail(IdempotencyApiErrors.HeaderRequired);
            return;
        }

        var idempotencyKey = keyValues.ToString().Trim();

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            context.Result = Fail(IdempotencyApiErrors.HeaderEmpty);
            return;
        }

        if (idempotencyKey.Length > 120)
        {
            context.Result = Fail(IdempotencyApiErrors.HeaderTooLong);
            return;
        }

        var userId = _userAccessor.UserId;
        if (userId is null)
        {
            context.Result = Fail(IdempotencyApiErrors.UserIdNotFound);
            return;
        }

        var operation = attribute.Operation;
        var now = DateTimeOffset.UtcNow;
        var ctn = context.HttpContext.RequestAborted;
        var requestHash = await ComputeRequestHashAsync(
            request,
            ctn);

        var existing = await _store.GetAsync(
            userId.Value,
            operation,
            idempotencyKey,
            ctn);

        if (existing is not null)
        {
            if (existing.IsExpired(now))
            {
                _logger.LogWarning(
                    "Idempotency key expired for Operation {Operation} and User {UserId}",
                    operation,
                    userId);

                context.Result = Fail(IdempotencyApiErrors.KeyExpired);
                return;
            }
            if (!existing.HasSameRequestHash(requestHash))
            {
                _logger.LogWarning(
                    "Idempotency key reused with different request for Operation {Operation} and User {UserId}",
                    operation,
                    userId);

                context.Result = Fail(IdempotencyApiErrors.KeyUsedWithDifferentRequest);
                return;
            }

            if (existing.Status == IdempotencyRequestStatus.Completed)
            {
                _logger.LogInformation(
                    "Idempotency cached response returned for Operation {Operation} and User {UserId}",
                    operation,
                    userId);

                context.Result = BuildCachedResult(existing);
                return;
            }

            if (existing.Status == IdempotencyRequestStatus.Processing)
            {
                _logger.LogWarning(
                    "Idempotency request already processing for Operation {Operation} and User {UserId}",
                    operation,
                    userId);

                context.Result = Fail(IdempotencyApiErrors.RequestAlreadyProcessing);
                return;
            }

            if (existing.Status == IdempotencyRequestStatus.Failed)
            {
                context.Result = Fail(IdempotencyApiErrors.PreviousRequestFailed);
                return;
            }
        }

        var record = IdempotencyRecord.Create(
            userId,
            operation,
            idempotencyKey,
            requestHash,
            now,
            now.AddMinutes(attribute.ExpirationMinutes));

        var created = await _store.TryBeginAsync(
            record,
            ctn);

        if (!created)
        {
            var duplicated = await _store.GetAsync(
                userId,
                operation,
                idempotencyKey,
                ctn);

            if (duplicated is not null)
            {
                if (!duplicated.HasSameRequestHash(requestHash))
                {
                    context.Result = Fail(IdempotencyApiErrors.KeyUsedWithDifferentRequest);
                    return;
                }

                if (duplicated.Status == IdempotencyRequestStatus.Completed)
                {
                    context.Result = BuildCachedResult(duplicated);
                    return;
                }

                if (duplicated.Status == IdempotencyRequestStatus.Failed)
                {
                    context.Result = Fail(IdempotencyApiErrors.PreviousRequestFailed);
                    return;
                }
            }

            context.Result = Fail(IdempotencyApiErrors.RequestAlreadyProcessing);
            return;
        }

        ResourceExecutedContext executedContext;

        try
        {
            executedContext = await next();
        }
        catch (Exception ex)
        {
            await _store.MarkFailedAsync(
                record.Id,
                ex.Message,
                CancellationToken.None);

            throw;
        }

        // Exception happened inside pipeline but wasn't thrown
        if (executedContext.Exception is not null)
        {
            await _store.MarkFailedAsync(
                record.Id,
                executedContext.Exception.Message,
                CancellationToken.None);

            return;
        }

        if (executedContext.Result is not IApiResult result)
        {
            await _store.MarkFailedAsync(
                record.Id,
                "Unsupported response type for idempotency caching.",
                CancellationToken.None);

            return;
        }

        if (!result.IsSuccess)
        {
            await _store.MarkFailedAsync(
                record.Id,
                result.Error?.Message ?? "Unknown error",
                CancellationToken.None);

            return;
        }

        var cachedResponse = ExtractResponse(executedContext.Result);

        if (!cachedResponse.ShouldCache)
        {
            await _store.MarkFailedAsync(
                record.Id,
                "Response type is not cacheable by the idempotency filter.",
                CancellationToken.None);

            return;
        }

        await _store.MarkCompletedAsync(
            record.Id,
            cachedResponse.StatusCode,
            cachedResponse.BodyJson,
            cachedResponse.ContentType,
            CancellationToken.None);
    }

    private static IActionResult Fail(Error error)
    {
        var statusCode = ApiResultMapper.MapStatusCode(error);

        var apiResponse = ApiResult.Fail(
            statusCode,
            error.Code,
            error.Message);

        return new ObjectResult(apiResponse.Response)
        {
            StatusCode = statusCode
        };
    }

    private static IActionResult BuildCachedResult(IdempotencyRecord record)
    {
        if (record.ResponseBodyJson is null)
        {
            return new StatusCodeResult(
                record.ResponseStatusCode ?? StatusCodes.Status200OK);
        }

        return new ContentResult
        {
            StatusCode = record.ResponseStatusCode ?? StatusCodes.Status200OK,
            ContentType = record.ContentType ?? "application/json",
            Content = record.ResponseBodyJson
        };
    }

    private static CachedResponse ExtractResponse(IActionResult? result)
    {
        if (result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

            if (statusCode < 200 || statusCode >= 300)
                return CachedResponse.NotCacheable();

            var json = JsonSerializer.Serialize(objectResult.Value);

            return CachedResponse.Cacheable(
                statusCode,
                json,
                "application/json");
        }

        if (result is JsonResult jsonResult)
        {
            var statusCode = jsonResult.StatusCode ?? StatusCodes.Status200OK;

            if (statusCode < 200 || statusCode >= 300)
                return CachedResponse.NotCacheable();

            var json = JsonSerializer.Serialize(jsonResult.Value);

            return CachedResponse.Cacheable(
                statusCode,
                json,
                "application/json");
        }

        if (result is StatusCodeResult statusCodeResult)
        {
            if (statusCodeResult.StatusCode < 200 || statusCodeResult.StatusCode >= 300)
                return CachedResponse.NotCacheable();

            return CachedResponse.Cacheable(
                statusCodeResult.StatusCode,
                null,
                "application/json");
        }

        return CachedResponse.NotCacheable();
    }

    private static async Task<string> ComputeRequestHashAsync(
        HttpRequest request,
        CancellationToken ct)
    {
        request.EnableBuffering();

        string body;

        using (var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true))
        {
            body = await reader.ReadToEndAsync(ct);
            request.Body.Position = 0;
        }

        var raw = $"{request.Method}|{request.Path}|{request.QueryString}|{body}";

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));

        return Convert.ToHexString(bytes);
    }

    private sealed record CachedResponse(
        bool ShouldCache,
        int StatusCode,
        string? BodyJson,
        string ContentType)
    {
        public static CachedResponse Cacheable(
            int statusCode,
            string? bodyJson,
            string contentType)
        {
            return new CachedResponse(
                true,
                statusCode,
                bodyJson,
                contentType);
        }

        public static CachedResponse NotCacheable()
        {
            return new CachedResponse(
                false,
                0,
                null,
                "application/json");
        }
    }
}
