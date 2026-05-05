using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Domain.Common.Errors;
using MediatR;
using AutoMapper;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using E_Commerce.Application.Features.Cart.Common;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem;

internal sealed class UpdateItemHandler : IRequestHandler<UpdateItemCommand, Result<CartSummaryDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICartSessionService _cartSessionService;
    private readonly IUserAccessor _userAccessor;
    private readonly IMapper _mapper;

    public UpdateItemHandler(
        IUnitOfWork uow,
        ICartSessionService cartSessionService,
        IUserAccessor userAccessor,
        IMapper mapper)
    {
        _uow = uow;
        _cartSessionService = cartSessionService;
        _userAccessor = userAccessor;
        _mapper = mapper;
    }

    public async Task<Result<CartSummaryDTO>> Handle(
       UpdateItemCommand request,
       CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var cart = await ResolveCartAsync(cancellationToken);

        if (cart is null)
            return Result<CartSummaryDTO>.Fail(CartErrors.NotActive);

        var item = cart.Items.FirstOrDefault(x => x.Id == request.cartItemId);

        if (item is null)
            return Result<CartSummaryDTO>.Fail(CartErrors.ItemNotFound);

        if (request.quantity == 0)
        {
            cart.RemoveItem(item.Id, now);
        }
        else
        {
            item.SetQuantity(request.quantity, now);
            cart.Touch(now);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        var cartSummary = _mapper.Map<CartSummaryDTO>(cart);

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