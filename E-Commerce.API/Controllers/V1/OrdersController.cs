using Asp.Versioning;
using E_Commerce.API.Common.Contracts;
using E_Commerce.API.Configuration;
using E_Commerce.API.Common.Pagination;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.OrderRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Order.Commands.CancelOrder;
using E_Commerce.Application.Features.Order.Commands.RetryOrderPayment;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Application.Features.Order.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.API.Controllers.V1
{
    [ApiVersion(1)]
    [ApiController()]
    [Authorize]
    [Route("api/v{version:apiVersion}/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _sender;

        public OrdersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<ApiResult<OrdersResponse>> GetOrders ([FromQuery] PageApiRequest page , CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new GetOrdersQuery(new(page.PageNumber , page.PageSize)), ctn), orders => new OrdersResponse(orders), "Orders retrieved successfully.");
        }

        [HttpGet("{orderId}")]
        public async Task<ApiResult<OrderResponse>> GetOrderById(Guid orderId, CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new GetOrderByIdQuery(orderId), ctn), order => new OrderResponse(order), "Order retrieved successfully.");
        }

        [HttpPost("{orderId}/cancel")]
        [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
        public async Task<ApiResult> CancelOrder(Guid orderId , [FromBody] CancelOrderRequest request, CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new CancelOrderCommand(orderId , request.reason), ctn), "Order canceled successfully.");
        }

        [HttpPost("{id:guid}/payment")]
        [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
        public async Task<ApiResult<PaymentResponse>> RetryPayment(Guid id, CancellationToken ctn)
        {
            return this.FromResult(
                await _sender.Send(new RetryOrderPaymentCommand(id), ctn),
                payment => new PaymentResponse(payment),
                "Payment session retrieved successfully.");
        }
    }
}
