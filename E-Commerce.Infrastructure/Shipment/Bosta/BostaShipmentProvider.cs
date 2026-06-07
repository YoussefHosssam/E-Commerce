using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Cache;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment.DTOs;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Infrastructure.Payment.Paymob;
using E_Commerce.Infrastructure.Settings;
using E_Commerce.Infrastructure.Shipment.Bosta.Contracts;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace E_Commerce.Infrastructure.Shipment.Bosta;

public sealed class BostaShipmentProvider : IShipmentProvider
{
    public string Provider {get;set;} = "Bosta";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BostaOptions _options;
    private readonly ILogger<BostaShipmentProvider> _logger;
    private readonly ILocalCacheService _localCacheService;

    public BostaShipmentProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<BostaOptions> options,
        ILogger<BostaShipmentProvider> logger,
        ILocalCacheService localCacheService)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _localCacheService = localCacheService;
    }

    public async Task<Result<decimal>> CalculateFees(
        ShipmentFeesRequest request,
        CancellationToken ct)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            string cacheFeesKey = BuildShipmentFeesCacheKey(_options.PickUpCity , request.Address.City!);
            decimal? fees = await _localCacheService.GetAsync<decimal?>(cacheFeesKey, ct);
            if (fees.HasValue)
                return Result<decimal>.Success(fees.Value);

            var bostaClient = _httpClientFactory.CreateClient(nameof(BostaClient));

            var queryParams = new Dictionary<string, string?>
            {
                ["cod"] = request.TotalPrice.ToString(),
                ["dropOfCity"] = request.Address.City,
                ["pickupCity"] = _options.PickUpCity,
                ["size"] = _options.Size,
                ["type"] = _options.Type
            };

            var url = QueryHelpers.AddQueryString(
                "api/v2/pricing/shipment/calculator",
                queryParams);

            _logger.LogInformation(
                "Calculating shipment fees with Provider {Provider} for City {City}",
                Provider,
                request.Address.City);

            using var response = await bostaClient.GetAsync(url, ct);

            var rawJson = await response.Content.ReadAsStringAsync(ct);

            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Shipment provider {Provider} returned HTTP {StatusCode} while calculating fees in {ElapsedMs} ms. Response: {Response}",
                    Provider,
                    (int)response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    rawJson);

                return Result<decimal>.Fail(ShipmentProviderErrors.RequestFailed);
            }

            using var docJson = JsonDocument.Parse(rawJson);

            if (!docJson.RootElement.TryGetProperty("success", out var successElement) ||
                !successElement.GetBoolean())
            {
                _logger.LogWarning(
                    "Shipment provider {Provider} returned unsuccessful response. Response: {Response}",
                    Provider,
                    rawJson);

                return Result<decimal>.Fail(ShipmentProviderErrors.RequestFailed);
            }

            if (!docJson.RootElement.TryGetProperty("data", out var dataElement) ||
                !dataElement.TryGetProperty("shippingFee", out var shippingFeeElement))
            {
                _logger.LogError(
                    "Shipment provider {Provider} returned invalid response shape. Response: {Response}",
                    Provider,
                    rawJson);

                return Result<decimal>.Fail(ShipmentProviderErrors.InvalidResponse);
            }

            var shippingFee = shippingFeeElement.GetDecimal();

            _logger.LogInformation(
                "Shipment provider {Provider} calculated fees successfully. Fee {ShippingFee}, elapsed {ElapsedMs} ms",
                Provider,
                shippingFee,
                stopwatch.ElapsedMilliseconds);

            var ttl = TimeSpan.FromMinutes(Random.Shared.Next(25, 35));
            await _localCacheService.SetAsync<decimal>(cacheFeesKey, shippingFee, ttl, ct);
            return Result<decimal>.Success(shippingFee);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "Shipment provider {Provider} request canceled after {ElapsedMs} ms",
                Provider,
                stopwatch.ElapsedMilliseconds);

            return Result<decimal>.Fail(ShipmentProviderErrors.RequestFailed);
        }
        catch (JsonException exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Shipment provider {Provider} response deserialization failed after {ElapsedMs} ms",
                Provider,
                stopwatch.ElapsedMilliseconds);

            return Result<decimal>.Fail(ShipmentProviderErrors.InvalidResponse);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Shipment provider {Provider} request failed after {ElapsedMs} ms",
                Provider,
                stopwatch.ElapsedMilliseconds);

            return Result<decimal>.Fail(ShipmentProviderErrors.ServiceUnavailable);
        }
    }
    private static string BuildShipmentFeesCacheKey(
    string pickupCity,
    string dropOffCity)
    {
        return $"bosta-shipment-fees:" +
               $"{pickupCity.Trim().ToUpperInvariant()}:" +
               $"{dropOffCity.Trim().ToUpperInvariant()}";
    }
}