using E_Commerce.Application.Common.Result;
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
using Microsoft.Extensions.Logging;
using E_Commerce.Application.Contracts.API.Identity;

namespace E_Commerce.Application.Features.Cart.Commands.AddItem
{
    internal class AddItemToCartHandler : IRequestHandler<AddItemToCartCommand, Result<CartSummaryDTO>>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICartSessionService _cartService;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<AddItemToCartHandler> _logger;

        public AddItemToCartHandler(
            IUnitOfWork uow,
            ICartSessionService cartService,
            IUserAccessor userAccessor,
            IMapper mapper,
            ILogger<AddItemToCartHandler> logger)
        {
            _uow = uow;
            _cartService = cartService;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<CartSummaryDTO>> Handle(
            AddItemToCartCommand request,
            CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var variant = await _uow.Variants.GetByIdWithDetailsAsync(
                request.variantId,
                cancellationToken);

            if (variant is null)
                return Result<CartSummaryDTO>.Fail(VariantErrors.NotFound);

            if (!variant.IsActive || !variant.Product.IsActive)
                return Result<CartSummaryDTO>.Fail(CheckoutErrors.InActiveProduct);

            var userId = _userAccessor.UserId;

            var cart = userId.HasValue
                ? await GetOrCreateUserCartAsync(userId.Value, now, cancellationToken)
                : await GetOrCreateAnonymousCartAsync(now, cancellationToken);

            return await AddItemToCartAsync(cart, request, now, cancellationToken);
        }

        private async Task<CartEntity> GetOrCreateUserCartAsync(
            Guid userId,
            DateTimeOffset now,
            CancellationToken ct)
        {
            var cart = await _uow.Carts.GetCartWithItemsByUserId(userId, ct);

            if (cart is not null)
                return cart;

            cart = CartEntity.CreateForUser(userId, now);

            await _uow.Carts.CreateAsync(cart, ct);

            return cart;
        }

        private async Task<CartEntity> GetOrCreateAnonymousCartAsync(
            DateTimeOffset now,
            CancellationToken ct)
        {
            var token = _cartService.GetAnonymousId();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var existingCart = await _uow.Carts.GetCartWithItemsByToken(token, ct);

                if (existingCart is not null)
                    return existingCart;
            }

            var newToken = _cartService.CreateAnonymousId();

            var cart = CartEntity.CreateAnonymous(newToken, now);

            await _uow.Carts.CreateAsync(cart, ct);

            return cart;
        }
        private async Task<Result<CartSummaryDTO>> AddItemToCartAsync(
            CartEntity cart,
            AddItemToCartCommand request,
            DateTimeOffset now,
            CancellationToken ct)
        {
            if (cart.GetTotalQuantity() + request.quantity > 20)
                return Result<CartSummaryDTO>.Fail(CartErrors.ItemsLimitExceeded);

            var isQuantityValid = await _uow.Inventories.IsQuantityValid(
                request.variantId,
                request.quantity,
                ct);

            if (!isQuantityValid)
                return Result<CartSummaryDTO>.Fail(CartItemErrors.QuantityInvalid);

            var cartItem = CartItem.Create(
                cart.Id,
                request.variantId,
                request.quantity,
                now);

            cart.AddItem(cartItem, now);

            await _uow.SaveChangesAsync(ct);

            var reloadedCart = cart.UserId.HasValue
                ? await _uow.Carts.GetCartWithItemsByUserId(cart.UserId.Value, ct)
                : await _uow.Carts.GetCartWithItemsByToken(cart.AnonymousToken!, ct);

            var cartSummary = _mapper.Map<CartSummaryDTO>(reloadedCart);

            return Result<CartSummaryDTO>.Success(cartSummary);
        }
    }
}
