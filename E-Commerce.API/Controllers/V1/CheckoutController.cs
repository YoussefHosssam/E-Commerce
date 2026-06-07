using Asp.Versioning;
using E_Commerce.API.Attributes;
using E_Commerce.API.Configuration;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CheckoutRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Features.Checkout.Commands;
using E_Commerce.Application.Features.Checkout.Common;
using E_Commerce.Application.Features.Checkout.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly ISender _sender;
        public CheckoutController(ISender sender)
        {
            _sender = sender;
        }
        [HttpGet("summary")]
        public async Task<ApiResult<CheckoutResponse>> GetCheckoutSummary(CancellationToken ct)
        {
            var result = await _sender.Send(new GetCheckoutSummaryQuery() , ct);
            return this.FromResult(result, checkout => new CheckoutResponse(checkout), "Checkout summary retrieved successfully.");
        }

        [HttpPost("review")]
        [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
        public async Task<ApiResult<CheckoutReviewResponse>> ReviewCheckout([FromBody] PlaceOrderRequest request, CancellationToken ct)
        {
            var result = await _sender.Send(new ReviewCheckoutQuery(request.DefaultAddress, request.AddressId, request.SameAsShipping, request.BillingAddress), ct);
            return this.FromResult(result, checkout => new CheckoutReviewResponse(checkout), "Checkout review retrieved successfully.");
        }

        [HttpPost("")]
        [Idempotent("checkout")]
        [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
        public async Task<ApiResult<OrderPlacementResponse>> PlaceOrder( [FromBody] PlaceOrderRequest request , CancellationToken ct)
        {
            var result = await _sender.Send(new PlaceOrderCommand(request.DefaultAddress , request.AddressId , request.SameAsShipping , request.BillingAddress), ct);
            return this.FromResult(result, order => new OrderPlacementResponse(order), "Order placed successfully.");
        }
    }
}
