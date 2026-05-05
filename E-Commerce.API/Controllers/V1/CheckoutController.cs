using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CheckoutRequests;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Application.Features.Checkout.Commands;
using E_Commerce.Application.Features.Checkout.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion(1)]
    [Route("/api/v{version:apiVersion}/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly ISender _sender;
        public CheckoutController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet("summary")]
        public async Task<ApiResult<CheckoutSummaryDto>> GetCheckoutSummary(CancellationToken ct)
        {
            var result = await _sender.Send(new GetCheckoutSummaryQuery() , ct);
            return this.FromResult(result, "Checkout summary retrieved successfully.");
        }

        [HttpPost("")]
        public async Task<ApiResult<PlaceOrderResponse>> PlaceOrder( [FromBody] PlaceOrderRequest request , CancellationToken ct)
        {
            var result = await _sender.Send(new PlaceOrderCommand(request.ShippingAddress , request.SameAsShipping , request.BillingAddress), ct);
            return this.FromResult(result, "Order placed successfully.");
        }
    }
}
