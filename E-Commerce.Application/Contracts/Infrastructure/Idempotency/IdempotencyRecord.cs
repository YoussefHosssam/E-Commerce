using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Idempotency
{
    public record IdempotencyRecord
    {
        public Guid UserId { get; private set; }
        public string Operation { get; private set; } = default!;
        public string IdempotencyKey { get; private set; } = default!;
        public string RequestHash { get; private set; } = default!;
        public IdempotencyStatus Status { get; private set; }
        public int? ResponseStatusCode { get; private set; }
        public string? ResponseBodyJson { get; private set; }
        public string? ContentType { get; private set; }

        [JsonConstructor]
        public IdempotencyRecord(
            Guid userId,
            string operation,
            string idempotencyKey,
            string requestHash,
            IdempotencyStatus status,
            int? responseStatusCode,
            string? responseBodyJson,
            string? contentType)
        {
            UserId = userId;
            Operation = operation;
            IdempotencyKey = idempotencyKey;
            RequestHash = requestHash;
            Status = status;
            ResponseStatusCode = responseStatusCode;
            ResponseBodyJson = responseBodyJson;
            ContentType = contentType;
        }

        private IdempotencyRecord(
            Guid userId,
            string operation,
            string idemKey,
            string reqHash)
        {
            UserId = userId;
            RequestHash = reqHash;
            IdempotencyKey = idemKey;
            Operation = operation;
            Status = IdempotencyStatus.Processing;
        }

        public static IdempotencyRecord Create(
            Guid userId,
            string operation,
            string idemKey,
            string reqHash)
        {
            return new IdempotencyRecord(userId, operation, idemKey, reqHash);
        }

        public bool HasSameRequestHash(string requestHash)
        {
            return string.Equals(RequestHash, requestHash);
        }

        public void MarkCompleted(
            int statusCode,
            string? responseBodyJson,
            string contentType)
        {
            Status = IdempotencyStatus.Completed;
            ResponseStatusCode = statusCode;
            ResponseBodyJson = responseBodyJson;
            ContentType = contentType;
        }

        public void MarkFailed()
        {
            Status = IdempotencyStatus.Failed;
        }
    }
}
