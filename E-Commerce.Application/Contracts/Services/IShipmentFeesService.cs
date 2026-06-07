using E_Commerce.Application.Common.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IShipmentFeesService
    {
        Task<Result<decimal>> CalculateShipmentFeeAsync(ResolvedCheckoutAddress address, decimal totalPrice, CancellationToken ctn);
    }
}
