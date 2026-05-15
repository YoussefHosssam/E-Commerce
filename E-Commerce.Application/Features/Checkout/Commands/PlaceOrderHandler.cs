using AutoMapper;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Services;
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
    private readonly IUserAccessor _userAccessor;
    private readonly IOrderService _orderService;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IMapper _mapper;
    private readonly ILogger<PlaceOrderHandler> _logger;

    public PlaceOrderHandler(
        IUnitOfWork uow,
        IUserAccessor userAccessor,
        IOrderService orderService,
        IPaymentGateway paymentGateway,
        IMapper mapper,
        ILogger<PlaceOrderHandler> logger)
    {
        _uow = uow;
        _userAccessor = userAccessor;
        _orderService = orderService;
        _paymentGateway = paymentGateway;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PlaceOrderResponse>> Handle(
        PlaceOrderCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var userResult = GetCurrentUserId();

        if (!userResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(userResult.Error!);

        var userId = userResult.Data;

        var cartResult = await GetAndValidateCartAsync(userId, cancellationToken);

        if (!cartResult.IsSuccess)
        {
            _logger.LogWarning(
                "Checkout blocked for User {UserId} with {ErrorCode}",
                userId,
                cartResult.Error?.Code);

            return Result<PlaceOrderResponse>.Fail(cartResult.Error!);
        }

        var cart = cartResult.Data!;
        var cartItems = cart.Items.ToList();

        var inventoryResult = await GetAndValidateInventoriesAsync(
            cartItems,
            cancellationToken);

        if (!inventoryResult.IsSuccess)
        {
            _logger.LogWarning(
                "Checkout inventory validation failed for User {UserId} with {ErrorCode}",
                userId,
                inventoryResult.Error?.Code);

            return Result<PlaceOrderResponse>.Fail(inventoryResult.Error!);
        }

        var inventoryByVariantId = inventoryResult.Data!;

        var orderResult = await CreateOrderAsync(
            request,
            userId,
            now,
            cancellationToken);

        if (!orderResult.IsSuccess)
            return Result<PlaceOrderResponse>.Fail(orderResult.Error!);

        var order = orderResult.Data!;

        await ReserveStockAndCreateOrderItemsAsync(
            order,
            cartItems,
            inventoryByVariantId,
            userId,
            now,
            cancellationToken);

        var paymentAttempt = await CreatePaymentAttemptAsync(
            order,
            now,
            cancellationToken);

        cart.SetStatus(CartStatus.CheckedOut, now);

        await _uow.SaveChangesAsync(cancellationToken);

        var providerRequest = BuildProviderPaymentSessionRequest(
            request,
            order,
            paymentAttempt,
            cartItems);

        var paymentDto = await TryInitializeProviderPaymentSessionAsync(
            order,
            paymentAttempt,
            providerRequest,
            cancellationToken);


        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Order {OrderId} created for User {UserId} with Total {GrandTotal} {Currency} and PaymentAttempt {PaymentAttemptId}",
            order.Id,
            userId,
            order.GrandTotal,
            order.Currency.Value,
            paymentAttempt.Id);

        return Result<PlaceOrderResponse>.Success(
            new PlaceOrderResponse(
                order.Id,
                order.OrderNumber,
                order.GrandTotal,
                order.Currency.Value,
                paymentDto));
    }

    private Result<Guid> GetCurrentUserId()
    {
        if (!_userAccessor.UserId.HasValue)
            return Result<Guid>.Fail(AuthErrors.InvalidToken);

        return Result<Guid>.Success(_userAccessor.UserId.Value);
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

            if (inventory.Available < item.Quantity)
                return Result<Dictionary<Guid, Inventory>>.Fail(CheckoutErrors.VariantOutOfStock);
        }

        return Result<Dictionary<Guid, Inventory>>.Success(inventoryByVariantId);
    }

    private async Task<Result<OrderEntity>> CreateOrderAsync(
        PlaceOrderCommand request,
        Guid userId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        return await _orderService.CreateOrder(
            userId,
            CurrencyCode.Create("EGP"),
            request.ShippingAddress,
            request.SameAsShipping,
            request.BillingAddress,
            cancellationToken,
            now);
    }

    private async Task ReserveStockAndCreateOrderItemsAsync(
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

            inventory.Reserve(item.Quantity, now);

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
        OrderEntity order,
        PaymentAttempt paymentAttempt,
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
                request.ShippingAddress,
                request.SameAsShipping,
                request.BillingAddress),
            IdempotencyKey: paymentAttempt.IdempotencyKey,
            SpecialReference: paymentAttempt.Id.ToString("N"),
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
                JsonText.Create("{}"));

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

                return new PaymentSessionItemDto(
                    Name: product.Slug.Value,
                    Amount: price.Amount,
                    Quantity: item.Quantity);
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
