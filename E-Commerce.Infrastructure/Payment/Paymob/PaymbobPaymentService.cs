using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Payment.Paymob
{
    internal sealed class PaymobPaymentGateway : IPaymentGateway
    {
        private readonly IUnitOfWork _uow;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PaymobPyamentOptions _options;
        private readonly ILogger<PaymobPaymentGateway> _logger;

        public PaymobPaymentGateway(
            IUnitOfWork uow,
            IOptions<PaymobPyamentOptions> options,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymobPaymentGateway> logger)
        {
            _uow = uow;
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

        public async Task<Result<CreateProviderPaymentSessionResult>> CreateSessionAsync(CreateProviderPaymentSessionRequest request, CancellationToken ct)
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

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/intention")
                {
                    Content = JsonContent.Create(new PaymobSessionRequest
                    {
                        billing_data = request.BillingData,
                        items = request.Items,
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

                    return Result<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedInitSession);
                }

                var paymobResponse = JsonSerializer.Deserialize<PaymobPaymentIntentionResponse>(
                    rawJson,
                    _jsonOptions);

                if (paymobResponse is null)
                {
                    _logger.LogError(
                        "Payment provider {Provider} response deserialization failed for Order {OrderId}, PaymentAttempt {PaymentAttemptId}",
                        Provider,
                        request.OrderId,
                        request.PaymentAttemptId);

                    return Result<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedDeserializeResponse);
                }

                var paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_options.PublicKey}&clientSecret={paymobResponse.ClientSecret}";

                var result = new CreateProviderPaymentSessionResult(Provider, paymobResponse.IntentionOrderId, paymobResponse.Id, paymentUrl, paymobResponse.ClientSecret, rawJson);

                _logger.LogInformation(
                    "Payment provider {Provider} session created for Order {OrderId}, PaymentAttempt {PaymentAttemptId} in {ElapsedMs} ms",
                    Provider,
                    request.OrderId,
                    request.PaymentAttemptId,
                    stopwatch.ElapsedMilliseconds);

                return Result<CreateProviderPaymentSessionResult>.Success(result);
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

                return Result<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.CancelRequest);
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

                return Result<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.FailedRequest);
            }
        }

        public Task<Result<ProviderWebhookEvent>> ParseAndVerifyWebhookAsync(string rawBody, string? hmac, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
