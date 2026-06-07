using AutoMapper;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment.DTOs;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Features.Checkout.Common;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using CartEntity = E_Commerce.Domain.Entities.Cart;
using OrderEntity = E_Commerce.Domain.Entities.Order;

namespace E_Commerce.Application.Features.Checkout.Commands;

internal sealed class PlaceOrderHandler
    : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponse>>
{
    private const int PaymentExpiryMinutes = 15;

    private readonly IUnitOfWork _uow;
    private readonly ICheckoutAddressResolver _addressResolver;
    private readonly IOrderService _orderService;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaceOrderHandler> _logger;
    private readonly ITransactionManager _transactionManager;
    private readonly IShipmentFeesService _shipmentFeesService;

    public PlaceOrderHandler(
        IUnitOfWork uow,
        ICheckoutAddressResolver addressResolver,
        IOrderService orderService,
        IPaymentGateway paymentGateway,
        IMapper mapper,
        ILogger<PlaceOrderHandler> logger,
        IShipmentFeesService shipmentFeesService,
        ITransactionManager transactionManager)
    {
        _uow = uow;
        _addressResolver = addressResolver;
        _orderService = orderService;
        _paymentGateway = paymentGateway;
        _mapper = mapper;
        _logger = logger;
        _shipmentFeesService = shipmentFeesService;
        _transactionManager = transactionManager;
    }

    public async Task<Result<PlaceOrderResponse>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var addressResult = await _addressResolver.ResolveAsync(
            new CheckoutAddressSelection(request.DefaultAddress, request.AddressId),
            cancellationToken);

        if (!addressResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(addressResult.Error!);

        var resolvedAddress = addressResult.Data!;
        var userId = resolvedAddress.User.Id;

        var cartResult = await GetAndValidateCartAsync(userId, cancellationToken);

        if (!cartResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(cartResult.Error!);

        var cart = cartResult.Data!;
        var cartItems = cart.Items.ToList();

        // External call OUTSIDE transaction
        var shipmentFeeResult = await _shipmentFeesService.CalculateShipmentFeeAsync(
            resolvedAddress,
            cart.GetTotalPrice(),
            cancellationToken);

        if (!shipmentFeeResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(shipmentFeeResult.Error!);

        decimal shipmentFee = shipmentFeeResult.Data!;
        OrderEntity order;
        PaymentAttempt paymentAttempt;

        var transactionResult = await _transactionManager.ExecuteAsync(async ct =>
        {
            var inventoryResult = await GetAndValidateInventoriesAsync(cartItems, ct);

            if (!inventoryResult.IsSuccess)
                return Result<(OrderEntity Order, PaymentAttempt PaymentAttempt)>
                    .Fail(inventoryResult.Error!);

            var inventoryByVariantId = inventoryResult.Data!;

            var orderResult = await CreateOrderAsync(
                request,
                resolvedAddress,
                shipmentFee,
                userId,
                now,
                ct);

            if (!orderResult.IsSuccess)
                return Result<(OrderEntity Order, PaymentAttempt PaymentAttempt)>
                    .Fail(orderResult.Error!);

            var createdOrder = orderResult.Data!;

            var reserveResult = await ReserveStockAndCreateOrderItemsAsync(
                createdOrder,
                cartItems,
                inventoryByVariantId,
                userId,
                now,
                ct);
            if (!reserveResult.IsSuccess)
                return Result<(OrderEntity Order, PaymentAttempt PaymentAttempt)>.Fail(reserveResult.Error!);

            var createdPaymentAttempt = await CreatePaymentAttemptAsync(
                createdOrder,
                now,
                ct);

            cart.SetStatus(CartStatus.CheckedOut, now);

            return Result<(OrderEntity Order, PaymentAttempt PaymentAttempt)>.Success(
                (createdOrder, createdPaymentAttempt));

        }, cancellationToken);

        if (!transactionResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(transactionResult.Error!);

        order = transactionResult.Data.Order;
        paymentAttempt = transactionResult.Data.PaymentAttempt;

        var providerRequest = BuildProviderPaymentSessionRequest(
            request,
            resolvedAddress,
            order,
            paymentAttempt,
            shippingFee:shipmentFee,
            cartItems);

        var paymentDto = await TryInitializeProviderPaymentSessionAsync(
            order,
            paymentAttempt,
            providerRequest,
            cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);
        if (paymentDto is null)
            return Result<PlaceOrderResponse>.Fail(InfrastructureErrors.PaymentGatewayFail);
        return Result<PlaceOrderResponse>.Success(
            new PlaceOrderResponse(
                order.Id,
                order.OrderNumber,
                order.GrandTotal,
                order.Currency.Value,
                paymentDto));
    }

    private async Task<Result<CartEntity>> GetAndValidateCartAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var cart = await _uow.Carts.GetCartWithItemsByUserId(
            userId,
            cancellationToken);

        if (cart is null || !cart.Items.Any())
            return Result<CartEntity>.Fail(CheckoutErrors.EmptyCart);

        var cartItems = cart.Items.ToList();

        if (cartItems.Any(i => i.Quantity <= 0))
            return Result<CartEntity>.Fail(CartItemErrors.QuantityInvalid);

        foreach (var item in cartItems)
        {
            if (item.Variant is null || item.Variant.Product is null)
                return Result<CartEntity>.Fail(VariantErrors.NotFound);

            if (!item.Variant.IsActive || !item.Variant.Product.IsActive)
                return Result<CartEntity>.Fail(CheckoutErrors.InActiveProduct);
        }

        return Result<CartEntity>.Success(cart);
    }

    private async Task<Result<Dictionary<Guid, Inventory>>> GetAndValidateInventoriesAsync(
        IReadOnlyCollection<CartItem> cartItems,
        CancellationToken cancellationToken)
    {
        var variantIds = cartItems
            .Select(i => i.VariantId)
            .Distinct()
            .ToList();

        var inventories = await _uow.Inventories.GetByVariantIdsAsync(
            variantIds,
            cancellationToken);

        var inventoryByVariantId = inventories.ToDictionary(i => i.VariantId);

        foreach (var item in cartItems)
        {
            if (!inventoryByVariantId.TryGetValue(item.VariantId, out var inventory))
                return Result<Dictionary<Guid, Inventory>>.Fail(CheckoutErrors.UnfoundInventory);

            // Friendly early check only
            if (inventory.Available < item.Quantity)
                return Result<Dictionary<Guid, Inventory>>.Fail(CheckoutErrors.VariantOutOfStock);
        }

        return Result<Dictionary<Guid, Inventory>>.Success(inventoryByVariantId);
    }

    private async Task<Result<OrderEntity>> CreateOrderAsync(
        PlaceOrderCommand request,
        ResolvedCheckoutAddress resolvedAddress,
        decimal shippingFee,
        Guid userId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrder(
            userId,
            CurrencyCode.Create("EGP"),
            resolvedAddress.ShippingAddress,
            request.SameAsShipping,
            request.BillingAddress,
            shippingFee,
            cancellationToken,
            now);
    }
    private async Task<Result> ReserveStockAndCreateOrderItemsAsync(
        OrderEntity order,
        IReadOnlyCollection<CartItem> cartItems,
        Dictionary<Guid, Inventory> inventoryByVariantId,
        Guid userId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var stockMovements = new List<StockMovement>();
        var orderItems = new List<OrderItem>();

        foreach (var item in cartItems)
        {
            var inventory = inventoryByVariantId[item.VariantId];
            var variant = item.Variant!;
            var product = variant.Product!;

            var isReserved = await _uow.Inventories.TryReserveAsync(variant.Id, item.Quantity, now, cancellationToken);
            if (!isReserved)
                return Result.Fail(InventoryErrors.ReservedQuantityInvalid);

            var movement = StockMovement.Create(
                item.VariantId,
                StockMovementType.Reservation,
                -item.Quantity,
                "Order stock reservation",
                order.Id,
                userId,
                now);

            stockMovements.Add(movement);
            VariantSnapshot variantSnapshot = _mapper.Map<VariantSnapshot>(variant);
            var orderItem = OrderItem.Create(
                order.Id,
                item.VariantId,
                variant.GetPrice().Currency,
                variant.Sku,
                product.Name,
                JsonText.Create(JsonSerializer.Serialize(variantSnapshot)),
                variant.GetPrice().Amount,
                item.Quantity);

            order.AddItem(orderItem , now);
        }

        await _uow.StockMovements.CreateRangeAsync(
            stockMovements,
            cancellationToken);
        return Result.Success();
    }

    private async Task<PaymentAttempt> CreatePaymentAttemptAsync(
        OrderEntity order,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var paymentExpiresAt = now.AddMinutes(PaymentExpiryMinutes);

        var paymentIdempotencyKey = $"checkout:{order.Id:N}:payment";

        var paymentAttempt = PaymentAttempt.Create(
            orderId: order.Id,
            provider: _paymentGateway.Provider,
            amount: order.GrandTotal,
            currency: order.Currency,
            idempotencyKey: paymentIdempotencyKey,
            expiresAt: paymentExpiresAt,
            now: now,
            requestHash: null,
            rawPayloadJson: JsonText.Create("{}"));

        await _uow.PaymentAttempts.CreateAsync(
            paymentAttempt,
            cancellationToken);

        return paymentAttempt;
    }

    private CreateProviderPaymentSessionRequest BuildProviderPaymentSessionRequest(
        PlaceOrderCommand request,
        ResolvedCheckoutAddress resolvedAddress,
        OrderEntity order,
        PaymentAttempt paymentAttempt,
        decimal shippingFee,
        IReadOnlyCollection<CartItem> cartItems)
    {
        return new CreateProviderPaymentSessionRequest(
            OrderId: order.Id,
            PaymentAttemptId: paymentAttempt.Id,
            OrderNumber: order.OrderNumber,
            Amount: order.GrandTotal,
            Currency: order.Currency,
            Items: BuildPaymentItems(cartItems),
            BillingData: BuildBillingData(
                resolvedAddress.ShippingAddress,
                request.SameAsShipping,
                request.BillingAddress),
            IdempotencyKey: paymentAttempt.IdempotencyKey,
            SpecialReference: paymentAttempt.Id.ToString("N"),
            ShippingFee:shippingFee,
            ExpiresAt: paymentAttempt.ExpiresAt);
    }

    private async Task<PaymentDto> TryInitializeProviderPaymentSessionAsync(
        OrderEntity order,
        PaymentAttempt paymentAttempt,
        CreateProviderPaymentSessionRequest providerRequest,
        CancellationToken cancellationToken)
    {
        var providerResult = await _paymentGateway.CreateSessionAsync(
            providerRequest,
            cancellationToken);

        if (!providerResult.IsSuccess)
        {
            _logger.LogWarning(
                "Payment session creation failed for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}, ErrorCode {ErrorCode}",
                order.Id,
                paymentAttempt.Id,
                paymentAttempt.Provider,
                providerResult.Error?.Code);

            paymentAttempt.MarkFailed(
                DateTimeOffset.UtcNow,
                JsonText.Create(providerResult.RawPayloadJson));

            return PaymentDto.FailedInitialization(
                provider: paymentAttempt.Provider,
                paymentAttemptId: paymentAttempt.Id,
                expiresAt: paymentAttempt.ExpiresAt);
        }

        var providerSession = providerResult.Data!;

        _logger.LogInformation(
            "Payment session created for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}",
            order.Id,
            paymentAttempt.Id,
            providerSession.Provider);

        paymentAttempt.AttachProviderSession(
            providerSessionId: providerSession.ProviderSessionId!,
            paymentUrl: providerSession.PaymentUrl ?? providerSession.ClientSecret!,
            now: DateTimeOffset.UtcNow,
            rawPayloadJson: JsonText.Create(providerSession.RawPayloadJson));

        if (!string.IsNullOrWhiteSpace(providerSession.ProviderOrderId))
        {
            paymentAttempt.AttachProviderOrderId(
                providerSession.ProviderOrderId,
                DateTimeOffset.UtcNow);
        }

        return PaymentDto.Created(
            paymentAttemptId: paymentAttempt.Id,
            provider: providerSession.Provider,
            paymentUrl: providerSession.PaymentUrl,
            clientSecret: providerSession.ClientSecret,
            expiresAt: paymentAttempt.ExpiresAt);
    }

    private static IReadOnlyCollection<PaymentSessionItemDto> BuildPaymentItems(
        IReadOnlyCollection<CartItem> cartItems)
    {
        return cartItems
            .Select(item =>
            {
                var variant = item.Variant!;
                var product = variant.Product!;
                var price = variant.GetPrice();
                var imageUrl = variant.GetPrimaryImage();

                return new PaymentSessionItemDto(
                    Name: product.Slug.Value,
                    Money: price,
                    Quantity: item.Quantity,
                    ImageUrl:imageUrl);
            })
            .ToList();
    }

    private static PaymentBillingDataDto BuildBillingData(
        ShippingAddressDto shippingAddress,
        bool sameAsShipping,
        BillingAddressDto? billingAddress)
    {
        if (sameAsShipping || billingAddress is null)
        {
            return new PaymentBillingDataDto(
                FirstName: shippingAddress.FirstName,
                LastName: shippingAddress.LastName,
                Email: shippingAddress.Email,
                PhoneNumber: shippingAddress.PhoneNumber,
                Street: shippingAddress.AddressLine1 ?? "NA",
                Building: "NA",
                Floor: "NA",
                Apartment: "NA",
                City: shippingAddress.City ?? "NA",
                State: "NA",
                Country: "EG",
                PostalCode: "NA");
        }

        return new PaymentBillingDataDto(
            FirstName: billingAddress.FirstName,
            LastName: billingAddress.LastName,
            Email: billingAddress.Email,
            PhoneNumber: billingAddress.PhoneNumber,
            Street: billingAddress.AddressLine1 ?? "NA",
            Building: "NA",
            Floor: "NA",
            Apartment: "NA",
            City: billingAddress.City ?? "NA",
            State: "NA",
            Country: "EG",
            PostalCode: "NA");
    }
}
