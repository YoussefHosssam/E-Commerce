using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Common.Errors.ErrorCodes;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using OrderEntity = E_Commerce.Domain.Entities.Order;

namespace E_Commerce.Application.Features.Checkout.Commands
{
    internal class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserAccessor _userAccessor;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public PlaceOrderHandler(IUnitOfWork uow, IUserAccessor userAccessor, IOrderService orderService, IPaymentService paymentService )
        {
            _uow = uow;
            _userAccessor = userAccessor;
            _orderService = orderService;
            _paymentService = paymentService;
        }

        public async Task<Result<PlaceOrderResponse>> Handle(PlaceOrderCommand request,CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            if (!_userAccessor.UserId.HasValue)
                return Result<PlaceOrderResponse>.Fail(AuthErrors.InvalidToken);

            var userId = _userAccessor.UserId.Value;

            var cart = await _uow.Carts.GetCartWithItemsByUserId(userId, cancellationToken);

            if (cart is null || !cart.Items.Any())
                return Result<PlaceOrderResponse>.Fail(CheckoutErrors.EmptyCart);

            var cartItems = cart.Items.ToList();

            if (cartItems.Any(i => i.Quantity <= 0))
                return Result<PlaceOrderResponse>.Fail(CartItemErrors.QuantityInvalid);

            foreach (var item in cartItems)
            {
                if (item.Variant is null || item.Variant.Product is null)
                    return Result<PlaceOrderResponse>.Fail(VariantErrors.NotFound);

                if (!item.Variant.IsActive || !item.Variant.Product.IsActive)
                    return Result<PlaceOrderResponse>.Fail(CheckoutErrors.InActiveProduct);
            }

            var variantIds = cartItems
                .Select(i => i.VariantId)
                .Distinct()
                .ToList();

            var inventories = await _uow.Inventories
                .GetByVariantIdsAsync(variantIds, cancellationToken);

            var inventoryByVariantId = inventories.ToDictionary(i => i.VariantId);

            foreach (var item in cartItems)
            {
                if (!inventoryByVariantId.TryGetValue(item.VariantId, out var inventory))
                    return Result<PlaceOrderResponse>.Fail(CheckoutErrors.UnfoundInventory);

                if (inventory.Available <= 0)
                    return Result<PlaceOrderResponse>.Fail(CheckoutErrors.VariantOutOfStock);

                if (inventory.Available < item.Quantity)
                    return Result<PlaceOrderResponse>.Fail(CheckoutErrors.VariantOutOfStock);
            }

            var orderResult = await _orderService.CreateOrder(
                userId,
                CurrencyCode.Create("EGP"),
                request.ShippingAddress,
                request.SameAsShipping,
                request.BillingAddress,
                cancellationToken,
                now);

            if (!orderResult.IsSuccess)
                return Result<PlaceOrderResponse>.Fail(orderResult.Error!);

            var order = orderResult.Data!;

            var stockMovements = new List<Domain.Entities.StockMovement>();

            foreach (var item in cartItems)
            {
                var inventory = inventoryByVariantId[item.VariantId];

                inventory.Reserve(item.Quantity, now);

                var movement = Domain.Entities.StockMovement.Create(
                    item.VariantId,
                    StockMovementType.Reservation,
                    -item.Quantity,
                    "Order stock reservation",
                    order.Id,
                    userId,
                    now);

                stockMovements.Add(movement);
            }

            await _uow.StockMovements.CreateRangeAsync(stockMovements, cancellationToken);

            var paymentResult = await _paymentService.InitPaymentSession(
                userId,
                order,
                cancellationToken,
                now);

            if (!paymentResult.IsSuccess)
                return Result<PlaceOrderResponse>.Fail(paymentResult.Error!);

            cart.SetStatus(CartStatus.CheckedOut, now);

            var paymentDto = paymentResult.Data!;

            await _uow.SaveChangesAsync(cancellationToken);

            return Result<PlaceOrderResponse>.Success(
                new PlaceOrderResponse(
                    order.Id,
                    order.OrderNumber,
                    order.GrandTotal,
                    order.Currency.Value,
                    paymentDto));
        }
    }
}
