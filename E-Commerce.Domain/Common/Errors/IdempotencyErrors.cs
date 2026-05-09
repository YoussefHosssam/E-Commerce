using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Common.Errors
{
    internal static class IdempotencyErrors
    {
        public static readonly Error OperationRequired =
            new(ErrorCodes.Idempotency.OperationRequired,
                "Idempotency operation is required.",
                ErrorType.Validation);

        public static readonly Error OperationTooLong =
            new(ErrorCodes.Idempotency.OperationTooLong,
                "Idempotency operation exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error IdempotencyKeyRequired =
            new(ErrorCodes.Idempotency.IdempotencyKeyRequired,
                "Idempotency key is required.",
                ErrorType.Validation);

        public static readonly Error IdempotencyKeyTooLong =
            new(ErrorCodes.Idempotency.IdempotencyKeyTooLong,
                "Idempotency key exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error RequestHashRequired =
            new(ErrorCodes.Idempotency.RequestHashRequired,
                "Request hash is required.",
                ErrorType.Validation);

        public static readonly Error RequestHashTooLong =
            new(ErrorCodes.Idempotency.RequestHashTooLong,
                "Request hash exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error ExpiresAtInvalid =
            new(ErrorCodes.Idempotency.ExpiresAtInvalid,
                "Idempotency expiration date/time is invalid.",
                ErrorType.Validation);

        public static readonly Error StatusInvalidTransition =
            new(ErrorCodes.Idempotency.StatusInvalidTransition,
                "Invalid idempotency record status transition.",
                ErrorType.Conflict);

        public static readonly Error ResponseStatusCodeInvalid =
            new(ErrorCodes.Idempotency.ResponseStatusCodeInvalid,
                "Response status code is invalid.",
                ErrorType.Validation);

        public static readonly Error ContentTypeTooLong =
            new(ErrorCodes.Idempotency.ContentTypeTooLong,
                "Content type exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error ResourceIdTooLong =
            new(ErrorCodes.Idempotency.ResourceIdTooLong,
                "Resource ID exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error FailureReasonRequired =
            new(ErrorCodes.Idempotency.FailureReasonRequired,
                "Failure reason is required.",
                ErrorType.Validation);

        public static readonly Error FailureReasonTooLong =
            new(ErrorCodes.Idempotency.FailureReasonTooLong,
                "Failure reason exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error NowRequired =
            new(ErrorCodes.Idempotency.NowRequired,
                "Current date/time is required.",
                ErrorType.Validation);
    }
}
