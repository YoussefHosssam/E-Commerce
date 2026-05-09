using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
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

        public PaymobPaymentGateway(IUnitOfWork uow, IOptions<PaymobPyamentOptions> options, IHttpClientFactory httpClientFactory)
        {
            _uow = uow;
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
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
            try
            {
                var httpClient = _httpClientFactory.CreateClient(nameof(PaymobClient));

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

                if (!response.IsSuccessStatusCode)
                {
                    return Result<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedInitSession);
                }

                var paymobResponse = JsonSerializer.Deserialize<PaymobPaymentIntentionResponse>(
                    rawJson,
                    _jsonOptions);

                if (paymobResponse is null)
                {
                    return Result<CreateProviderPaymentSessionResult>.Fail(PaymentErrors.FailedDeserializeResponse);
                }

                var paymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_options.PublicKey}&clientSecret={paymobResponse.ClientSecret}";

                var result = new CreateProviderPaymentSessionResult(Provider, paymobResponse.IntentionOrderId, paymobResponse.Id, paymentUrl, paymobResponse.ClientSecret, rawJson);

                return Result<CreateProviderPaymentSessionResult>.Success(result);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return Result<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.CancelRequest);
            }
            catch (Exception ex)
            {
                return Result<CreateProviderPaymentSessionResult>.Fail(PaymobErrors.FailedRequest);
            }
        }

        public Task<Result<ProviderWebhookEvent>> ParseAndVerifyWebhookAsync(string rawBody, string? hmac, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
