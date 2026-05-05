using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly OrderNumberGenerator _orderNumberGenerator;

        public OrderService(IUnitOfWork uow, OrderNumberGenerator orderNumberGenerator)
        {
            _uow = uow;
            _orderNumberGenerator = orderNumberGenerator;
        }

        public async Task<Result<Order>> CreateOrder(Guid userId, CurrencyCode currency, ShippingAddressDto shippingAddress, bool isSameAddress, BillingAddressDto? billingAddress, CancellationToken ctn, DateTimeOffset now)
        {
            string orderNumber = _orderNumberGenerator.Generate();
            string shippingAddressJsonString = JsonSerializer.Serialize(shippingAddress);
            string billingAddressJsonString = JsonSerializer.Serialize(billingAddress);
            JsonText shippingAddressJSON = JsonText.Create(shippingAddressJsonString);
            JsonText BillingAddressJSON = JsonText.Create(billingAddressJsonString);
            Order order = Order.Create(userId, orderNumber, currency, shippingAddressJSON, BillingAddressJSON, string.Empty, now);
            await _uow.Orders.CreateAsync(order, ctn);
            await _uow.SaveChangesAsync(ctn);
            return Result<Order>.Success(order);
        }
    }
}
