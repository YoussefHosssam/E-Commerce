using System.Collections.Concurrent;
using System.Threading.Tasks;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Services;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderEntity = E_Commerce.Domain.Entities.Order;

namespace E_Commerce.Application.Features.Order.Commands.RetryOrderPayment;

public sealed class RetryOrderPaymentHandler
    : IRequestHandler<RetryOrderPaymentCommand, Result<PaymentDto>>
{
    private const int PaymentExpiryMinutes = 15;

    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> OrderLocks = new();

    private readonly IUnitOfWork _uow;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUserAccessor _userAccessor;
    private readonly ILogger<RetryOrderPaymentHandler> _logger;

    public RetryOrderPaymentHandler(
        IUnitOfWork uow,
        IPaymentGateway paymentGateway,
        IUserAccessor userAccessor,
        ILogger<RetryOrderPaymentHandler> logger)
    {
        _uow = uow;
        _paymentGateway = paymentGateway;
        _logger = logger;
        _userAccessor = userAccessor;
    }

    public async Task<Result<PaymentDto>> Handle(
        RetryOrderPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();

        _logger.LogInformation(
            "Payment retry requested for Order {OrderId} by User {UserId}",
            request.OrderId,
            userId);

        var orderLock = OrderLocks.GetOrAdd(request.OrderId, _ => new SemaphoreSlim(1, 1));
        await orderLock.WaitAsync(cancellationToken);



        try
        {
            return await HandleLockedAsync(
                request.OrderId,
                userId,
                cancellationToken);
        }
        finally
        {
            orderLock.Release();
        }
    }

    private async Task<Result<PaymentDto>> HandleLockedAsync(
        Guid orderId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        var order = await _uow.Orders.GetTrackingOrderByIdWithDetailsAsync(
            orderId,
            cancellationToken);

        if (order is null || order.UserId != userId)
            return Result<PaymentDto>.Fail(OrderErrors.NotFound);

        var payableResult = ValidatePayable(order);
        if (!payableResult.IsSuccess)
        {
            _logger.LogWarning(
                "Order {OrderId} is not payable for User {UserId}; Status {Status}; ErrorCode {ErrorCode}",
                order.Id,
                userId,
                order.Status,
                payableResult.Error?.Code);

            return Result<PaymentDto>.Fail(payableResult.Error!);
        }

        var activeAttempt = await _uow.PaymentAttempts.GetActivePaymentAttemptAsync(
            order.Id,
            now,
            cancellationToken);

        if (activeAttempt is not null)
        {
            _logger.LogInformation(
                "Existing active payment session reused for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}, Status {Status}",
                order.Id,
                activeAttempt.Id,
                activeAttempt.Provider,
                activeAttempt.Status);

            return Result<PaymentDto>.Success(ToReusedPaymentDto(activeAttempt));
        }

        await MarkLatestAttemptExpiredIfNeededAsync(
            order.Id,
            now,
            cancellationToken);

        var attemptCount = await _uow.PaymentAttempts.CountByOrderIdAsync(
            order.Id,
            cancellationToken);

        var paymentAttempt = PaymentAttempt.Create(
            orderId: order.Id,
            provider: _paymentGateway.Provider,
            amount: order.GrandTotal,
            currency: order.Currency,
            idempotencyKey: BuildIdempotencyKey(order.Id, attemptCount + 1),
            expiresAt: now.AddMinutes(PaymentExpiryMinutes),
            now: now,
            requestHash: null,
            rawPayloadJson: JsonText.Create("{}"));

        await _uow.PaymentAttempts.CreateAsync(
            paymentAttempt,
            cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "New payment attempt created for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}",
            order.Id,
            paymentAttempt.Id,
            paymentAttempt.Provider);

        var shippingAddress = JsonText.To<ShippingAddressDto>(order.ShippingAddressJson);

        var providerRequest = BuildProviderRequest(order, paymentAttempt , order.ShippingFee);
        var providerResult = await _paymentGateway.CreateSessionAsync(
            providerRequest,
            cancellationToken);

        if (!providerResult.IsSuccess || providerResult.Data is null)
        {
            _logger.LogWarning(
                "Payment provider initialization failed for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}, ErrorCode {ErrorCode}",
                order.Id,
                paymentAttempt.Id,
                paymentAttempt.Provider,
                providerResult.Error?.Code);

            paymentAttempt.MarkFailed(DateTimeOffset.UtcNow, JsonText.Create("{}"));
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<PaymentDto>.Fail(PaymentErrors.InitializationFailed);
        }

        var providerSession = providerResult.Data;
        if (string.IsNullOrWhiteSpace(providerSession.ProviderSessionId) ||
            (string.IsNullOrWhiteSpace(providerSession.PaymentUrl) &&
             string.IsNullOrWhiteSpace(providerSession.ClientSecret)))
        {
            _logger.LogWarning(
                "Payment provider returned invalid session for Order {OrderId}, PaymentAttempt {PaymentAttemptId}, Provider {Provider}",
                order.Id,
                paymentAttempt.Id,
                paymentAttempt.Provider);

            paymentAttempt.MarkFailed(DateTimeOffset.UtcNow, JsonText.Create(providerSession.RawPayloadJson));
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<PaymentDto>.Fail(PaymentErrors.InitializationFailed);
        }

        paymentAttempt.AttachProviderSession(
            providerSession.ProviderSessionId,
            providerSession.PaymentUrl ?? providerSession.ClientSecret!,
            DateTimeOffset.UtcNow,
            JsonText.Create(providerSession.RawPayloadJson));

        if (!string.IsNullOrWhiteSpace(providerSession.ProviderOrderId))
        {
            paymentAttempt.AttachProviderOrderId(
                providerSession.ProviderOrderId,
                DateTimeOffset.UtcNow);
        }

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<PaymentDto>.Success(
            PaymentDto.Created(
                paymentAttempt.Id,
                providerSession.Provider,
                providerSession.PaymentUrl,
                providerSession.ClientSecret,
                paymentAttempt.ExpiresAt));
    }

    private static Result ValidatePayable(OrderEntity order)
    {
        if (order.GrandTotal <= 0)
            return Result.Fail(OrderErrors.NoPayableAmount);

        return order.Status switch
        {
            OrderStatus.Pending or OrderStatus.PaymentFailed => Result.Success(),
            OrderStatus.Paid => Result.Fail(OrderErrors.AlreadyPaid),
            _ => Result.Fail(OrderErrors.PaymentNotAllowed)
        };
    }

    private async Task MarkLatestAttemptExpiredIfNeededAsync(
        Guid orderId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var latestAttempt = await _uow.PaymentAttempts.GetLatestPaymentAttemptAsync(
            orderId,
            cancellationToken);

        if (latestAttempt is null ||
            latestAttempt.ExpiresAt > now ||
            latestAttempt.Status is PaymentAttemptStatus.Paid or PaymentAttemptStatus.Failed or PaymentAttemptStatus.Expired)
        {
            return;
        }

        latestAttempt.MarkExpired(now);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    private static PaymentDto ToReusedPaymentDto(PaymentAttempt paymentAttempt)
    {
        return PaymentDto.Reused(
            paymentAttempt.Id,
            paymentAttempt.Provider,
            paymentAttempt.PaymentUrl,
            null,
            paymentAttempt.ExpiresAt,
            paymentAttempt.Status);
    }

    private static CreateProviderPaymentSessionRequest BuildProviderRequest(
        OrderEntity order,
        PaymentAttempt paymentAttempt,
        decimal shipmentFees)
    {
        var shippingAddress = JsonText.To<ShippingAddressDto>(order.ShippingAddressJson);

        return new CreateProviderPaymentSessionRequest(
            OrderId: order.Id,
            PaymentAttemptId: paymentAttempt.Id,
            OrderNumber: order.OrderNumber,
            Amount: order.GrandTotal,
            Currency: order.Currency,
            Items: BuildPaymentItems(order.Items),
            BillingData: BuildBillingData(shippingAddress),
            IdempotencyKey: paymentAttempt.IdempotencyKey,
            SpecialReference: paymentAttempt.Id.ToString("N"),
            shipmentFees,
            ExpiresAt: paymentAttempt.ExpiresAt);
    }

    private static IReadOnlyCollection<PaymentSessionItemDto> BuildPaymentItems(
        IReadOnlyCollection<OrderItem> orderItems)
    {
        return orderItems
            .Select(item => new PaymentSessionItemDto(item.ProductTitleSnapshot , Money.Create(item.UnitPrice , item.Currency) , item.Quantity)
            ).ToList();
    }

    private static PaymentBillingDataDto BuildBillingData(ShippingAddressDto shippingAddress)
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

    private static string BuildIdempotencyKey(Guid orderId, int attemptNumber)
        => $"order-payment:{orderId:N}:{attemptNumber:0000}";
}
