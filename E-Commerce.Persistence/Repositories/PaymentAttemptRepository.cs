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
    }
}
