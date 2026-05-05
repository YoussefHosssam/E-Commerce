using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using System.Threading.Tasks;
using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Features.Cart.Commands.RemoveItem
{
    public class RemoveItemHandler : IRequestHandler<RemoveItemCommand, Result>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly ICartSessionService _cartSessionService;
        public RemoveItemHandler(IUnitOfWork uow, IUserAccessor userAccessor, ICartSessionService cartService)
        {
            _uow = uow;
            _userAccessor = userAccessor;
            _cartSessionService = cartService;
        }
        public async Task<Result> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var cart = await ResolveCartAsync(cancellationToken);
            if (cart is null) return Result.Fail(CartErrors.NotActive);
            var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == request.cartItemId);
            if (cartItem is null) return Result.Fail(CartErrors.ItemNotFound);
            cart.RemoveItem(request.cartItemId , now);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result.Success();
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
