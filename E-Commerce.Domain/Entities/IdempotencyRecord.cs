using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;
using E_Commerce.Domain.Exceptions;

namespace E_Commerce.Domain.Entities;

public sealed class IdempotencyRecord : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Operation { get; private set; } = default!;
    public string IdempotencyKey { get; private set; } = default!;
    public string RequestHash { get; private set; } = default!;

    public IdempotencyRequestStatus Status { get; private set; }

    public int? ResponseStatusCode { get; private set; }
    public string? ResponseBodyJson { get; private set; }
    public string? ContentType { get; private set; }

    public string? ResourceId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    private IdempotencyRecord() { } // EF

    private IdempotencyRecord(
        Guid? userId,
        string operation,
        string idempotencyKey,
        string requestHash,
        DateTimeOffset now,
        DateTimeOffset expiresAt)
    {
        UserId = userId;
        Operation = operation;
        IdempotencyKey = idempotencyKey;
        RequestHash = requestHash;
        CreatedAt = now;
        ExpiresAt = expiresAt;
        Status = IdempotencyRequestStatus.Processing;
    }

    public static IdempotencyRecord Create(
        Guid? userId,
        string operation,
        string idempotencyKey,
        string requestHash,
        DateTimeOffset now,
        DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new DomainValidationException(IdempotencyErrors.OperationRequired);

        operation = operation.Trim();

        if (operation.Length > 100)
            throw new DomainValidationException(IdempotencyErrors.OperationTooLong);

        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainValidationException(IdempotencyErrors.IdempotencyKeyRequired);

        idempotencyKey = idempotencyKey.Trim();

        if (idempotencyKey.Length > 120)
            throw new DomainValidationException(IdempotencyErrors.IdempotencyKeyTooLong);

        if (string.IsNullOrWhiteSpace(requestHash))
            throw new DomainValidationException(IdempotencyErrors.RequestHashRequired);

        requestHash = requestHash.Trim();

        if (requestHash.Length > 128)
            throw new DomainValidationException(IdempotencyErrors.RequestHashTooLong);

        if (now == default)
            throw new DomainValidationException(IdempotencyErrors.NowRequired);

        if (expiresAt <= now)
            throw new DomainValidationException(IdempotencyErrors.ExpiresAtInvalid);

        return new IdempotencyRecord(
            userId,
            operation,
            idempotencyKey,
            requestHash,
            now,
            expiresAt);
    }

    public void MarkCompleted(
        int responseStatusCode,
        string? responseBodyJson,
        string? contentType,
        string? resourceId,
        DateTimeOffset now)
    {
        if (Status != IdempotencyRequestStatus.Processing)
            throw new DomainValidationException(IdempotencyErrors.StatusInvalidTransition);

        if (responseStatusCode < 200 || responseStatusCode > 599)
            throw new DomainValidationException(IdempotencyErrors.ResponseStatusCodeInvalid);

        if (!string.IsNullOrWhiteSpace(contentType) && contentType.Length > 100)
            throw new DomainValidationException(IdempotencyErrors.ContentTypeTooLong);

        if (!string.IsNullOrWhiteSpace(resourceId) && resourceId.Length > 120)
            throw new DomainValidationException(IdempotencyErrors.ResourceIdTooLong);

        if (now == default)
            throw new DomainValidationException(IdempotencyErrors.NowRequired);

        ResponseStatusCode = responseStatusCode;
        ResponseBodyJson = responseBodyJson;
        ContentType = string.IsNullOrWhiteSpace(contentType)
            ? "application/json"
            : contentType.Trim();

        ResourceId = string.IsNullOrWhiteSpace(resourceId)
            ? null
            : resourceId.Trim();

        Status = IdempotencyRequestStatus.Completed;
        CompletedAt = now;
        FailureReason = null;
    }

    public void MarkFailed(string failureReason, DateTimeOffset now)
    {
        if (Status != IdempotencyRequestStatus.Processing)
            throw new DomainValidationException(IdempotencyErrors.StatusInvalidTransition);

        if (string.IsNullOrWhiteSpace(failureReason))
            throw new DomainValidationException(IdempotencyErrors.FailureReasonRequired);

        failureReason = failureReason.Trim();

        if (failureReason.Length > 1000)
            throw new DomainValidationException(IdempotencyErrors.FailureReasonTooLong);

        if (now == default)
            throw new DomainValidationException(IdempotencyErrors.NowRequired);

        FailureReason = failureReason;
        Status = IdempotencyRequestStatus.Failed;
        CompletedAt = now;
    }

    public bool IsExpired(DateTimeOffset now)
    {
        if (now == default)
            throw new DomainValidationException(IdempotencyErrors.NowRequired);

        return ExpiresAt <= now;
    }

    public bool HasSameRequestHash(string requestHash)
    {
        if (string.IsNullOrWhiteSpace(requestHash))
            return false;

        return RequestHash == requestHash.Trim();
    }
}