using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Features.Checkout.Commands
{
    public record PlaceOrderCommand (ShippingAddressDto ShippingAddress, bool SameAsShipping, BillingAddressDto? BillingAddress) : IRequest<Result<PlaceOrderResponse>>
    {
    }

}
