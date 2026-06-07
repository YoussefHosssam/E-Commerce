using Asp.Versioning;
using E_Commerce.API.Common.Errors;
using E_Commerce.API.Common.Responses;
using E_Commerce.Application.Features.Payment.Commands;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/webhooks")]
    public class WebhooksController : ControllerBase
    {

        private readonly ISender _sender;
        public WebhooksController(ISender sender)
        {
            _sender = sender;
        }


        [AllowAnonymous]
        [HttpPost("payments/{gateway}")]
        public async Task<ApiResult> ParseAndVerifyPaymentWebhook(string gateway , CancellationToken ct)
        {
            if (!Enum.TryParse(gateway ,true, out PaymentGateway paymentGateway))
            {
                var err = ApiErrors.InvalidRequest;
                return ApiResult.Fail(400 , err.Code , err.Message);
            }
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync(ct);
            Console.WriteLine(rawBody);
            var headers = Request.Headers.ToDictionary(
                h => h.Key,
                h => h.Value.ToString());

            var queries = Request.Query.ToDictionary(
                h => h.Key,
                h => h.Value.ToString());

            await _sender.Send(new PaymentWebhookCommand(paymentGateway , new(rawBody , headers , queries)), ct);

            return ApiResult.Success("Payment parsing done successfully.");
        }
    }
}
