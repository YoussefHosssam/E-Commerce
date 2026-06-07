using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence
{
    public interface IPaymentAttemptRepository : IGenericRepository<PaymentAttempt>
    {
        Task<PaymentAttempt?> GetActivePaymentAttemptAsync(
            Guid orderId,
            DateTimeOffset now,
            CancellationToken ct);

        Task<PaymentAttempt?> GetLatestPaymentAttemptAsync(
            Guid orderId,
            CancellationToken ct);

        Task<int> CountByOrderIdAsync(
            Guid orderId,
            CancellationToken ct);

        public Task<PaymentAttempt?> GetPaymentAttemptByProviderOrderIdAsync(string providerOrderId, CancellationToken ct);

    }
}
