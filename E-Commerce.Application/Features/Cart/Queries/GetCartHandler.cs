using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Features.Cart.Common;
using MediatR;

namespace E_Commerce.Application.Features.Cart.Queries;

public class GetCartHandler : IRequestHandler<GetCartQuery, Result<CartSummaryDTO>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICartSessionService _cartSessionService;
    private readonly IUserAccessor _userAccessor;

    public GetCartHandler(IUnitOfWork uow, ICartSessionService cartSessionService, IUserAccessor userAccessor)
    {
        _uow = uow;
        _cartSessionService = cartSessionService;
        _userAccessor = userAccessor;
    }

    public async Task<Result<CartSummaryDTO>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cartSummary = await ResolveCartSummaryAsync(cancellationToken);
        return Result<CartSummaryDTO>.Success(cartSummary ?? CartSummaryDTO.Empty());
    }

    private async Task<CartSummaryDTO?> ResolveCartSummaryAsync(CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;

        if (userId.HasValue)
        {
            return await _uow.Carts.GetCartSummaryDtoByUserIdAsync(
                userId.Value,
                cancellationToken);
        }

        var anonymousToken = _cartSessionService.GetAnonymousId();

        if (string.IsNullOrWhiteSpace(anonymousToken))
            return null;

        return await _uow.Carts.GetCartSummaryDtoByTokenAsync(
            anonymousToken,
            cancellationToken);
    }
}
