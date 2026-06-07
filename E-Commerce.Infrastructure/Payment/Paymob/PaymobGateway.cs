using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Results;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Payment.Paymob
{
    internal sealed class PaymobGateway : IPaymentGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PaymobOptions _options;
        private readonly ILogger<PaymobGateway> _logger;

        public PaymobGateway(
            IOptions<PaymobOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymobGateway> logger)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
        public string Provider { get { return "Paymob"; } }

        public async Task<ProviderResult<CreateProviderPaymentSessionResult>> CreateSessionAsync(CreateProviderPaymentSessionRequest request,CancellationToken ct)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(PaymobClient));

                _logger.LogInformation(
                    "Creating payment session with Provider {Provider} for Order {OrderId}, PaymentAttempt {PaymentAttemptId}",
                    Provider,
                    request.OrderId,
                    request.PaymentAttemptId);
                var items = request.Items
                    .Select(i => new PaymobItem
                    {
                        Name = i.Name,
                        Amount = ToCents(i.Money.Amount),
                        Quantity = i.Quantity,
                        Image = i.ImageUrl
                    })
                    .ToList();

                if (request.ShippingFee > 0)
                {
                    items.Add(new PaymobItem
                    {
                        Name = "Shipping Fee",
                        Amount = ToCents(request.ShippingFee),
                        Quantity = 1
                    });
                }

                IReadOnlyCollection<PaymobItem> paymobItems = items.AsReadOnly(); using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/intention")
                {
                    Content = JsonContent.Create(new PaymobSessionRequest
                    {
                        billing_data = request.BillingData,
                        items = items,
                        currency = request.Currency.Value,
                        payment_methods = _options.PaymentMethods,
                        special_reference = request.OrderNumber
                    })
                };

                using var response = await httpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);

                var rawJson = await response.Content.ReadAsStringAsync(ct);
                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Payment provider {Provider} returned HTTP {StatusCode} for Order {OrderId}, PaymentAttempt {PaymentAttemptId} in {ElapsedMs} ms",
                        Provider,
                        (int)response.StatusCode,
                        request.OrderId,
                        request.PaymentAttemptId,
                        stopwatch.ElapsedMilliseconds);

                    return ProviderResult<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedInitSession , rawJson);
                }

                var paymobResponse = JsonSerializer.Deserialize<PaymobPaymentIntentionResponse>(
                    rawJson,
                    _jsonOptions);

                _logger.LogDebug(rawJson);

                if (paymobResponse is null)
                {
                    _logger.LogError(
                        "Payment provider {Provider} response deserialization failed for Order {OrderId}, PaymentAttempt {PaymentAttemptId}",
                        Provider,
                        request.OrderId,
                        request.PaymentAttemptId);

                    return ProviderResult<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedDeserializeResponse , rawJson);
                }

                var paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_options.PublicKey}&clientSecret={paymobResponse.ClientSecret}";

                var result = new CreateProviderPaymentSessionResult(Provider, Convert.ToString(paymobResponse.IntentionOrderId), paymobResponse.Id, paymentUrl, paymobResponse.ClientSecret, rawJson);

                _logger.LogInformation(
                    "Payment provider {Provider} session created for Order {OrderId}, PaymentAttempt {PaymentAttemptId} in {ElapsedMs} ms",
                    Provider,
                    request.OrderId,
                    request.PaymentAttemptId,
                    stopwatch.ElapsedMilliseconds);

                return ProviderResult<CreateProviderPaymentSessionResult>.Success(result , rawJson);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                stopwatch.Stop();

                _logger.LogWarning(
                    "Payment provider {Provider} request canceled for Order {OrderId}, PaymentAttempt {PaymentAttemptId} after {ElapsedMs} ms",
                    Provider,
                    request.OrderId,
                    request.PaymentAttemptId,
                    stopwatch.ElapsedMilliseconds);

                return ProviderResult<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.CancelRequest);
            }
            catch (Exception exception)
            {
                stopwatch.Stop();

                _logger.LogError(
                    exception,
                    "Payment provider {Provider} request failed for Order {OrderId}, PaymentAttempt {PaymentAttemptId} after {ElapsedMs} ms",
                    Provider,
                    request.OrderId,
                    request.PaymentAttemptId,
                    stopwatch.ElapsedMilliseconds);

                return ProviderResult<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.FailedRequest);
            }
        }

        public ProviderResult<ProviderWebhookEvent> ParseAndVerifyWebhook(
            WebhookRequest request)
        {
            if (!request.queries.TryGetValue("hmac", out var receivedHmac) ||
                string.IsNullOrWhiteSpace(receivedHmac))
            {
                return ProviderResult<ProviderWebhookEvent>.Fail(PaymobErrors.MissingWebhookHmac);
            }

            using var document = JsonDocument.Parse(request.rawBody);
            var root = document.RootElement;

            var obj = root.GetProperty("obj");

            var concatenated = string.Concat(
                Get(obj, "amount_cents"),
                Get(obj, "created_at"),
                Get(obj, "currency"),
                Get(obj, "error_occured"),
                Get(obj, "has_parent_transaction"),
                Get(obj, "id"),
                Get(obj, "integration_id"),
                Get(obj, "is_3d_secure"),
                Get(obj, "is_auth"),
                Get(obj, "is_capture"),
                Get(obj, "is_refunded"),
                Get(obj, "is_standalone_payment"),
                Get(obj, "is_voided"),
                Get(obj.GetProperty("order"), "id"),
                Get(obj, "owner"),
                Get(obj, "pending"),
                GetNullable(obj, "source_data", "pan"),
                GetNullable(obj, "source_data", "sub_type"),
                GetNullable(obj, "source_data", "type"),
                Get(obj, "success")
            );

            var calculatedHmac = CalculateHmacSha512(concatenated, _options.HMAC);

            if (!FixedTimeEquals(calculatedHmac, receivedHmac))
            {
                return ProviderResult<ProviderWebhookEvent>.Fail(PaymobErrors.InvalidWebhookHmac);
            }

            var webhook = JsonSerializer.Deserialize<PaymobWebhookRequest>(
                request.rawBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (webhook?.Obj is null)
            {
                return ProviderResult<ProviderWebhookEvent>.Fail(PaymobErrors.InvalidWebhookPayload);
            }

            var tx = webhook.Obj;

            var providerEvent = new ProviderWebhookEvent(
                Provider: "Paymob",
                EventType: webhook.Type,
                ProviderTransactionId: tx.Id.ToString(CultureInfo.InvariantCulture),
                ProviderOrderId: tx.Order.Id.ToString(CultureInfo.InvariantCulture),
                ProviderSessionId: null,
                MerchantOrderId: tx.Order.MerchantOrderId,
                Success: tx.Success,
                Pending: tx.Pending,
                IsRefund: tx.IsRefunded,
                IsVoid: tx.IsVoided,
                IsAuth: GetBool(obj, "is_auth"),
                IsCapture: GetBool(obj, "is_capture"),
                Amount: tx.AmountCents / 100m,
                Currency: CurrencyCode.Create(tx.Currency),
                RawPayloadJson: request.rawBody
            );

            return ProviderResult<ProviderWebhookEvent>.Success(providerEvent , request.rawBody);
        }

        private static string Get(JsonElement element, string propertyName)
        {
            return NormalizeValue(element.GetProperty(propertyName));
        }

        private static string GetNullable(JsonElement element, string parent, string child)
        {
            if (!element.TryGetProperty(parent, out var parentElement) ||
                parentElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                !parentElement.TryGetProperty(child, out var childElement))
            {
                return string.Empty;
            }

            return NormalizeValue(childElement);
        }

        private static bool GetBool(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var value) &&
                   value.ValueKind == JsonValueKind.True;
        }

        private static string NormalizeValue(JsonElement value)
        {
            return value.ValueKind switch
            {
                JsonValueKind.String => value.GetString() ?? string.Empty,
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Number => value.GetRawText(),
                JsonValueKind.Null => string.Empty,
                JsonValueKind.Undefined => string.Empty,
                _ => value.GetRawText()
            };
        }

        private static string CalculateHmacSha512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var aBytes = Encoding.UTF8.GetBytes(a.Trim().ToLowerInvariant());
            var bBytes = Encoding.UTF8.GetBytes(b.Trim().ToLowerInvariant());

            return aBytes.Length == bBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
        }
        private static int ToCents(decimal amount)
        {
            return (int)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
        }
    }
}
