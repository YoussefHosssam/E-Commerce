using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories.Shared
{
    internal sealed class EfTransactionManager : ITransactionManager
    {
        private readonly EcommerceContext _context;

        public EfTransactionManager(EcommerceContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(ct);

                try
                {
                    await operation(ct);

                    await _context.SaveChangesAsync(ct);

                    await transaction.CommitAsync(ct);
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public async Task<T> ExecuteAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken ct = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(ct);

                try
                {
                    var result = await operation(ct);

                    await _context.SaveChangesAsync(ct);

                    await transaction.CommitAsync(ct);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }
    }
}
