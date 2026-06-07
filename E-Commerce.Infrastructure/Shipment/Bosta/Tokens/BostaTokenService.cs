using E_Commerce.Application.Contracts.Infrastructure.Cache;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Infrastructure.Exceptions;
using E_Commerce.Infrastructure.Settings;
using E_Commerce.Infrastructure.Shipment.Bosta.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

public sealed class BostaTokenService : IBostaTokenService
{
    private const string AccessTokenCacheKey = "bosta:access-token";
    private const string RefreshTokenCacheKey = "bosta:refresh-token";

    private readonly HttpClient _httpClient;
    private readonly BostaOptions _options;
    private readonly ILocalCacheService _cache;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<BostaTokenService> _logger;

    public BostaTokenService(
        HttpClient httpClient,
        IOptions<BostaOptions> options,
        ILocalCacheService cache,
        ILogger<BostaTokenService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(
        CancellationToken cancellationToken = default)
    {
        var cachedAccessToken =
            await _cache.GetAsync<string>(AccessTokenCacheKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedAccessToken))
            return NormalizeBearerToken(cachedAccessToken);

        await _lock.WaitAsync(cancellationToken);

        try
        {
            cachedAccessToken =
                await _cache.GetAsync<string>(AccessTokenCacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(cachedAccessToken))
                return NormalizeBearerToken(cachedAccessToken);

            await LoginAsync(cancellationToken);

            cachedAccessToken =
                await _cache.GetAsync<string>(AccessTokenCacheKey, cancellationToken);

            return NormalizeBearerToken(cachedAccessToken!);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RefreshTokenAsync(
        CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            var refreshToken =
                await _cache.GetAsync<string>(RefreshTokenCacheKey, cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await LoginAsync(cancellationToken);
                return;
            }

            var request = new
            {
                refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/v2/users/refresh-token",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await ClearCacheAsync(cancellationToken);
                throw new InfrastructureException(ShipmentProviderErrors.TokenRefreshFailed);
            }

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = ReadTokenResponse(rawJson);

            await CacheTokensAsync(tokenResponse, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = new
        {
            email = _options.Email,
            password = _options.Password
        };

        try
        {
            _logger.LogInformation("Authenticating with Bosta");

            var response = await _httpClient.PostAsJsonAsync(
                "api/v2/users/login",
                request,
                cancellationToken);

            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Bosta authentication failed with HTTP {StatusCode} in {ElapsedMs} ms. Response: {Response}",
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    rawJson);

                throw new InfrastructureException(ShipmentProviderErrors.AuthenticationFailed);
            }

            var tokenResponse = ReadTokenResponse(rawJson);

            await CacheTokensAsync(tokenResponse, cancellationToken);

            _logger.LogInformation(
                "Bosta authentication succeeded in {ElapsedMs} ms",
                stopwatch.ElapsedMilliseconds);
        }
        catch (InfrastructureException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Bosta authentication response deserialization failed after {ElapsedMs} ms",
                stopwatch.ElapsedMilliseconds);

            throw new InfrastructureException(ShipmentProviderErrors.InvalidResponse);
        }
        catch (HttpRequestException exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Bosta authentication HTTP request failed after {ElapsedMs} ms",
                stopwatch.ElapsedMilliseconds);

            throw new InfrastructureException(ShipmentProviderErrors.ServiceUnavailable);
        }
        catch (TaskCanceledException exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Bosta authentication timed out/canceled after {ElapsedMs} ms",
                stopwatch.ElapsedMilliseconds);

            throw new InfrastructureException(ShipmentProviderErrors.ServiceUnavailable);
        }
    }
    private static BostaTokenResponse ReadTokenResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("data", out var data))
            throw new InfrastructureException(ShipmentProviderErrors.InvalidResponse);

        var accessToken = data.TryGetProperty("token", out var tokenElement)
            ? tokenElement.GetString()
            : null;

        var refreshToken = data.TryGetProperty("refreshToken", out var refreshTokenElement)
            ? refreshTokenElement.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new InfrastructureException(ShipmentProviderErrors.AccessTokenMissing);

        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new InfrastructureException(ShipmentProviderErrors.RefreshTokenMissing);

        return new BostaTokenResponse(accessToken, refreshToken);
    }

    private async Task CacheTokensAsync(
        BostaTokenResponse tokenResponse,
        CancellationToken cancellationToken)
    {
        await _cache.SetAsync(
            AccessTokenCacheKey,
            NormalizeBearerToken(tokenResponse.AccessToken),
            TimeSpan.FromMinutes(55),
            cancellationToken);

        await _cache.SetAsync(
            RefreshTokenCacheKey,
            tokenResponse.RefreshToken,
            TimeSpan.FromDays(7),
            cancellationToken);
    }

    private async Task ClearCacheAsync(CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(AccessTokenCacheKey, cancellationToken);
        await _cache.RemoveAsync(RefreshTokenCacheKey, cancellationToken);
    }

    private static string NormalizeBearerToken(string token)
    {
        return token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? token["Bearer ".Length..].Trim()
            : token.Trim();
    }

    private sealed record BostaTokenResponse(
        string AccessToken,
        string RefreshToken);
}