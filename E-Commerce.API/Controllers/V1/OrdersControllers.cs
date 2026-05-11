using Asp.Versioning;
using E_Commerce.API.Common.Contracts;
using E_Commerce.API.Common.Responses;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Order.Commands.CancelOrder;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Application.Features.Order.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1
{
    [ApiVersion(1)]
    [Route("/api/v{version:apiVersion}/orders")]
    public class OrdersControllers : ControllerBase
    {
        private readonly ISender _sender;

        public OrdersControllers(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet()]
        public async Task<ApiResult<List<OrderListDto>>> GetOrders ([FromQuery] PageRequest page , CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new GetOrdersQuery(page), ctn), "Orders retrieved successfully.");
        }

        [HttpGet("{orderId}")]
        public async Task<ApiResult<OrderDto>> GetOrderById(Guid orderId, CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new GetOrderByIdQuery(orderId), ctn), "Order retrieved successfully.");
        }

        [HttpPost("{orderId}/cancel")]
        public async Task<ApiResult> CancelOrder(Guid orderId, CancellationToken ctn)
        {
            return this.FromResult(await _sender.Send(new CancelOrderCommand(orderId), ctn), "Order canceled successfully.");
        }
    }
}
