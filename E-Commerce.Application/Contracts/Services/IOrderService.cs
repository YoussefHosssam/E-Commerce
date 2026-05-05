using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Services
{
    internal interface IOrderService
    {
        Task<Result<Order>> CreateOrder(Guid userId ,CurrencyCode currency ,ShippingAddressDto shippingAddress, bool isSameAddress, BillingAddressDto? billingAddress , CancellationToken ctn , DateTimeOffset now);
    }
}
