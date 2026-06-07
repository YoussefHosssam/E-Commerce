using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    internal class PaymentAttemptRepository : GenericRepository<PaymentAttempt> , IPaymentAttemptRepository
    {
        private readonly DbSet<PaymentAttempt> _paymentAttempts;
        public PaymentAttemptRepository(EcommerceContext ctx) : base(ctx)
        {
            _paymentAttempts = ctx.PaymentAttempts;        
        }

        public Task<PaymentAttempt?> GetActivePaymentAttemptAsync(
            Guid orderId,
            DateTimeOffset now,
            CancellationToken ct)
        {
            return _paymentAttempts
                .Where(x =>
                    x.OrderId == orderId &&
                    (x.Status == PaymentAttemptStatus.Initiated ||
                     x.Status == PaymentAttemptStatus.AwaitingCustomerAction) &&
                    x.ExpiresAt > now &&
                    x.PaymentUrl != null &&
                    x.PaymentUrl != string.Empty)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        public Task<PaymentAttempt?> GetLatestPaymentAttemptAsync(
            Guid orderId,
            CancellationToken ct)
        {
            return _paymentAttempts
                .Where(x => x.OrderId == orderId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }

        public Task<PaymentAttempt?> GetPaymentAttemptByProviderOrderIdAsync(string providerOrderId,CancellationToken ct)
        {
            return _paymentAttempts
                .Where(x => x.ProviderOrderId == providerOrderId && x.Status == PaymentAttemptStatus.AwaitingCustomerAction)
                .FirstOrDefaultAsync(ct);
        }
        public Task<int> CountByOrderIdAsync(
            Guid orderId,
            CancellationToken ct)
        {
            return _paymentAttempts.CountAsync(x => x.OrderId == orderId, ct);
        }
    }
}
