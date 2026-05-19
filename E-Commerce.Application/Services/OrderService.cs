using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System.Text.Json;

namespace E_Commerce.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly OrderNumberGenerator _orderNumberGenerator;

    public OrderService(
        IUnitOfWork uow,
        OrderNumberGenerator orderNumberGenerator)
    {
        _uow = uow;
        _orderNumberGenerator = orderNumberGenerator;
    }

    public async Task<Result<Order>> CreateOrder(
        Guid userId,
        CurrencyCode currency,
        ShippingAddressDto shippingAddress,
        bool isSameAddress,
        BillingAddressDto? billingAddress,
        decimal shippingFee,
        CancellationToken ct,
        DateTimeOffset now)
    {
        if (userId == Guid.Empty)
            return Result<Order>.Fail(OrderErrors.UserIdRequired);

        if (now == default)
            return Result<Order>.Fail(OrderErrors.NowRequired);

        string orderNumber = _orderNumberGenerator.Generate();

        JsonText shippingAddressJson = JsonText.From(shippingAddress);

        JsonText billingAddressJson = isSameAddress
            ? shippingAddressJson
            : JsonText.From(billingAddress!);

        Order order = Order.Create(
            userId,
            orderNumber,
            currency,
            shippingAddressJson,
            billingAddressJson,
            string.Empty,
            now);

        order.SetShippingFee(shippingFee, now);

        await _uow.Orders.CreateAsync(order, ct);


        return Result<Order>.Success(order);
    }
}
