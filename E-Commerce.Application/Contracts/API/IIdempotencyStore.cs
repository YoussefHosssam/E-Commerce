using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.API
{
    public interface IIdempotencyStore
    {
        Task<IdempotencyRecord?> GetAsync(
            Guid? userId,
            string operation,
            string idempotencyKey,
            CancellationToken ct);

        Task<bool> TryBeginAsync(
            IdempotencyRecord record,
            CancellationToken ct);

        Task MarkCompletedAsync(
            Guid recordId,
            int statusCode,
            string? responseBodyJson,
            string contentType,
            CancellationToken ct);

        Task MarkFailedAsync(
            Guid recordId,
            string failureReason,
            CancellationToken ct);
    }
}
