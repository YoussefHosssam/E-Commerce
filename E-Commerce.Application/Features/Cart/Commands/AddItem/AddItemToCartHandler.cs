using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using E_Commerce.Application.Features.Cart.Common;

namespace E_Commerce.Application.Features.Cart.Commands.AddItem
{
    internal class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result<CartSummaryDTO>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICartSessionService _cartService;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public AddItemToCartHandler(IUnitOfWork uow, ICartSessionService cartService, IUserAccessor userAccessor, IMapper mapper)
        {
            _uow = uow;
            _cartService = cartService;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<Result<CartSummaryDTO>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            Guid? userId = _userAccessor.UserId;
            if (!await _uow.Variants.VariantExistsAsync(request.variantId, cancellationToken)) return Result<CartSummaryDTO>.Fail(CartItemErrors.VariantIdRequired);
            if (!await _uow.Inventories.IsQuantityValid(request.variantId , request.quantity, cancellationToken)) return Result<CartSummaryDTO>.Fail(CartItemErrors.QuantityInvalid);
            if (userId.HasValue) return await HandleAddingToAuthenticatedCart(request, userId, now, cancellationToken);
            string? cartSessionId = _cartService.GetAnonymousId();
            if (cartSessionId == null) return await HandleAddingToNewCart(request, cancellationToken , now);
            else return await HandleAddingExistingCart(cartSessionId , request, cancellationToken , now);
        }

        private async Task<Result<CartSummaryDTO>> HandleAddingToAuthenticatedCart(AddItemToCartCommand request, Guid? userId, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var cart = await _uow.Carts.GetCartWithItemsByUserId(userId!.Value, cancellationToken);
            if (cart is null) return await HandleAddingToNewCart(request, cancellationToken, now);
            return await AddToCartService(cart, request, cancellationToken, now);
        }

        private async Task<Result<CartSummaryDTO>> HandleAddingExistingCart(string cartSessionId , AddItemToCartCommand request, CancellationToken cancellationToken , DateTimeOffset now)
        {
            var cart = await _uow.Carts.GetCartWithItemsByToken(cartSessionId, cancellationToken);
            if (cart is null) return Result<CartSummaryDTO>.Fail(CartErrors.NotActive);
            return await AddToCartService(cart, request, cancellationToken, now);
        }

        private async Task<Result<CartSummaryDTO>> HandleAddingToNewCart(AddItemToCartCommand request, CancellationToken cancellationToken , DateTimeOffset now)
        {
            var cartSessionToken = _cartService.CreateAnonymousId();
            Guid? userId = _userAccessor.UserId;
            if (userId is null)
            {
                CartEntity cart = CartEntity.CreateAnonymous(cartSessionToken, now);
                await _uow.Carts.CreateAsync(cart, cancellationToken);
                return await AddToCartService(cart, request, cancellationToken, now);
            }
            else
            {
                CartEntity cart = CartEntity.CreateForUser(userId.Value, now);
                await _uow.Carts.CreateAsync(cart, cancellationToken);
                return await AddToCartService(cart, request, cancellationToken, now);
            }
        }
        private async Task<Result<CartSummaryDTO>> AddToCartService(CartEntity cart , AddItemToCartCommand request , CancellationToken cancellationToken, DateTimeOffset now)
        {
            var cartQuantity = cart.GetTotalQuantity();
            if (cartQuantity + request.quantity > 20) return Result<CartSummaryDTO>.Fail(CartErrors.ItemsLimitExceeded);
            CartItem cartItem = CartItem.Create(cart.Id, request.variantId, request.quantity, now);
            if (!await _uow.Inventories.IsQuantityValid(request.variantId, cartItem.Quantity, cancellationToken)) return Result<CartSummaryDTO>.Fail(CartItemErrors.QuantityInvalid);
            cart.AddItem(cartItem, now);
            await _uow.SaveChangesAsync(cancellationToken);
            var reloadedCart = cart.UserId.HasValue
            ? await _uow.Carts.GetCartWithItemsByUserId(cart.UserId.Value, cancellationToken)
            : await _uow.Carts.GetCartWithItemsByToken(cart.AnonymousToken!, cancellationToken);
            if (reloadedCart is null)
                return Result<CartSummaryDTO>.Fail(CartErrors.NotActive);

            var cartSummary = _mapper.Map<CartSummaryDTO>(reloadedCart);

            return Result<CartSummaryDTO>.Success(cartSummary);
        }
    }
}
