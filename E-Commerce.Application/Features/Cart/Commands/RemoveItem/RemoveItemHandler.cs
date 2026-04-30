using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Domain.Common.Errors;
using MediatR;
using CartEntity = E_Commerce.Domain.Entities.Cart;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem;

internal sealed class RemoveItemHandler : IRequestHandler<RemoveItemCommand, Result<CartSummaryDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICartSessionService _cartSessionService;
    private readonly IUserAccessor _userAccessor;

    public RemoveItemHandler(
        IUnitOfWork uow,
        ICartSessionService cartSessionService,
        IUserAccessor userAccessor)
    {
        _uow = uow;
        _cartSessionService = cartSessionService;
        _userAccessor = userAccessor;
    }

    public async Task<Result<CartSummaryDTO>> Handle(
        RemoveItemCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var cart = await ResolveCartAsync(cancellationToken);

        if (cart is null)
            return Result<CartSummaryDTO>.Fail(
                ErrorCatalog.FromCode(ErrorCodes.Cart.NotActive));

        var item = cart.Items.FirstOrDefault(x => x.VariantId == request.variantId);

        if (item is null)
            return Result<CartSummaryDTO>.Fail(
                ErrorCatalog.FromCode(ErrorCodes.Cart.ItemNotFound));

        if (request.quantity >= item.Quantity)
        {
            cart.RemoveItem(request.variantId, now);
        }
        else
        {
            item.SetQuantity(item.Quantity - request.quantity, now);
            cart.Touch(now);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        var cartSummary = new CartSummaryDTO(
            cart.Id,
            cart.Items.ToList(),
            cart.GetTotalQuantity(),
            cart.GetTotalPrice(),
            cart.AnonymousToken);

        return Result<CartSummaryDTO>.Success(cartSummary);
    }

    private async Task<CartEntity?> ResolveCartAsync(CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        if (userId.HasValue)
        {
            return await _uow.Carts.GetCartWithItemsByUserId(
                userId.Value,
                cancellationToken);
        }

        var anonymousToken = _cartSessionService.GetAnonymousId();

        if (string.IsNullOrWhiteSpace(anonymousToken))
            return null;

        return await _uow.Carts.GetCartWithItemsByToken(
            anonymousToken,
            cancellationToken);
    }
}