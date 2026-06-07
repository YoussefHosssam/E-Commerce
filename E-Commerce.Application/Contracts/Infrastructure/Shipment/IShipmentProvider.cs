using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Shipment.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Shipment
{
    public interface IShipmentProvider
    {
        string Provider { get; set; }
        public Task<Result<decimal>> CalculateFees(ShipmentFeesRequest request , CancellationToken ctn);
    }
}
