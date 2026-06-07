using E_Commerce.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Shipment.DTOs
{
    public sealed record ShipmentFeesRequest(ShippingAddressDto Address , decimal TotalPrice);
}
