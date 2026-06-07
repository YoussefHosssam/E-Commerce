using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CartRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Features.Cart.Commands.AddItem;
using E_Commerce.Application.Features.Cart.Commands.RemoveItem;
using E_Commerce.Application.Features.Cart.Common;
using E_Commerce.Application.Features.Cart.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<ApiResult<CartResponse>> GetCart(CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new GetCartQuery(), ct),
            cart => new CartResponse(cart),
            "Cart retrieved successfully.");

    [HttpPost("items")]
    public async Task<ApiResult<CartResponse>> AddItemToCart(
        [FromBody] AddItemRequest request,
        CancellationToken ct)
    {
        var command = new AddItemToCartCommand(
            request.VariantId,
            request.Quantity);

        return this.FromResult(
            await _sender.Send(command, ct),
            cart => new CartResponse(cart),
            "Item added to cart successfully.",
            StatusCodes.Status201Created);
    }

    [HttpPatch("items/{cartItemId:guid}")]
    public async Task<ApiResult<CartResponse>> EditItemOnCart(
        Guid cartItemId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken ct)
    {
        var command = new UpdateItemCommand(
            cartItemId,
            request.Quantity);

        return this.FromResult(
            await _sender.Send(command, ct),
            cart => new CartResponse(cart),
            "Cart item updated successfully.");
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<ApiResult> RemoveItemFromCart(
        Guid cartItemId,
        CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new RemoveItemCommand(cartItemId), ct),
            "Item removed from cart successfully.");

}
