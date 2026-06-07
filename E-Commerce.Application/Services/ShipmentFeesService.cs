using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment.DTOs;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Common.Errors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class ShipmentFeesService : IShipmentFeesService
    {
        private readonly IShipmentProvider _shipmentFeesProvider;
        private readonly ILogger<ShipmentFeesService> _logger;
        public ShipmentFeesService(IShipmentProvider shipmentFeesProvider, ILogger<ShipmentFeesService> logger)
        {
            _shipmentFeesProvider = shipmentFeesProvider;
            _logger = logger;
        }
        public async Task<Result<decimal>> CalculateShipmentFeeAsync(ResolvedCheckoutAddress resolvedAddress, decimal totalPrice , CancellationToken ctn)
        {
            try
            {
                var shipmentRequest = new ShipmentFeesRequest(resolvedAddress.ShippingAddress, totalPrice);
                var result = await _shipmentFeesProvider.CalculateFees(shipmentRequest, ctn);

                if (result is null || !result.IsSuccess || result.Data < 0)
                    return Result<decimal>.Fail(CheckoutErrors.ShipmentFeeCalculationFailed);

                return Result<decimal>.Success(result.Data);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Shipment fee calculation failed for User {UserId} Address {AddressId}",
                    resolvedAddress.User.Id,
                    resolvedAddress.Address.Id);

                return Result<decimal>.Fail(CheckoutErrors.ShipmentFeeCalculationFailed);
            }
        }
    }
}
