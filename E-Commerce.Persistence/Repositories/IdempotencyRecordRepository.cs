using E_Commerce.Application.Contracts.Persistence;
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
    internal sealed class IdempotencyRecordRepository : GenericRepository<IdempotencyRecord> , IIdempotencyRecordRepository
    {
        private readonly DbSet<IdempotencyRecord> _idempotencyRecords;
        public IdempotencyRecordRepository(EcommerceContext context) : base(context)
        {
            _idempotencyRecords = context.IdempotencyRecords;
        }
        public Task<IdempotencyRecord?> GetAsync(
            Guid? userId,
            string operation,
            string idempotencyKey,
            CancellationToken ct)
        {
            operation = operation.Trim();
            idempotencyKey = idempotencyKey.Trim();

            return _idempotencyRecords
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Operation == operation &&
                    x.IdempotencyKey == idempotencyKey,
                    ct);
        }
    }
}
