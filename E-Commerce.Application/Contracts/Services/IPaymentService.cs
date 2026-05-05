using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Checkout;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Services
{
    public interface IPaymentService
    {
        Task<Result<PaymentDto>> InitPaymentSession(Guid userId , Order order , CancellationToken ctn , DateTimeOffset now);
    }
}
