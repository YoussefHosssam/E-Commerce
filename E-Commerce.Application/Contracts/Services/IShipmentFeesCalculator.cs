using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IShipmentFeesCalculator
    {
        public Task<Result<decimal>> CalculateFees(ShippingAddressDto shippingAddress , bool isSame , BillingAddressDto? billingAddress = null);
    }
}
