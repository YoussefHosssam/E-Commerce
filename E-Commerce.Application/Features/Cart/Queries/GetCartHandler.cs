using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Features.Cart.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using System.Threading.Tasks;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Features.Cart.Queries
{
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
            var cart = await ResolveCartAsync(cancellationToken);
            if (cart is null) return Result<CartSummaryDTO>.Success(CartSummaryDTO.Empty());
            CartSummaryDTO cartSummary = new CartSummaryDTO(cart.Id, cart.Items.ToCartItemListDTO(), cart.GetTotalQuantity(), cart.GetTotalPrice(), cart.AnonymousToken);
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
}
