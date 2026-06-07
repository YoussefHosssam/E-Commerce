using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Idempotency
{
    public interface IIdempotencyStore
    {
        Task<IdempotencyRecord?> GetAsync(
            Guid userId,
            string operation,
            string idempotencyKey,
            CancellationToken ct);

        Task<bool> TryBeginAsync(
            IdempotencyRecord record,
            CancellationToken ct);

        Task MarkCompletedAsync(
            Guid userId,
            string operation,
            string idempotencyKey,
            int statusCode,
            string? responseBodyJson,
            string contentType,
            CancellationToken ct);

        Task MarkFailedAsync(
            Guid userId,
            string operation,
            string idempotencyKey,
            string failureReason,
            CancellationToken ct);
    }
}
