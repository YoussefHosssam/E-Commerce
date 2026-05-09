using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Common.Errors
{
    public static class PaymentAttemptErrors
    {
        public static readonly Error OrderIdRequired =
        new(ErrorCodes.PaymentAttempt.OrderIdRequired,
            "Order ID is required to create a payment attempt.",
            ErrorType.Validation);

        public static readonly Error ProviderRequired =
            new(ErrorCodes.PaymentAttempt.ProviderRequired,
                "Payment provider is required.",
                ErrorType.Validation);

        public static readonly Error ProviderTooLong =
            new(ErrorCodes.PaymentAttempt.ProviderTooLong,
                "Payment provider exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error AmountInvalid =
            new(ErrorCodes.PaymentAttempt.AmountInvalid,
                "Payment amount must be greater than zero.",
                ErrorType.Validation);

        public static readonly Error CurrencyRequired =
            new(ErrorCodes.PaymentAttempt.CurrencyRequired,
                "Currency is required.",
                ErrorType.Validation);

        public static readonly Error IdempotencyKeyRequired =
            new(ErrorCodes.PaymentAttempt.IdempotencyKeyRequired,
                "Idempotency key is required.",
                ErrorType.Validation);

        public static readonly Error IdempotencyKeyTooLong =
            new(ErrorCodes.PaymentAttempt.IdempotencyKeyTooLong,
                "Idempotency key exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error RequestHashTooLong =
            new(ErrorCodes.PaymentAttempt.RequestHashTooLong,
                "Request hash exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error ExpiresAtInvalid =
            new(ErrorCodes.PaymentAttempt.ExpiresAtInvalid,
                "Expiration date/time is invalid.",
                ErrorType.Validation);

        public static readonly Error ProviderSessionIdRequired =
            new(ErrorCodes.PaymentAttempt.ProviderSessionIdRequired,
                "Provider session ID is required.",
                ErrorType.Validation);

        public static readonly Error ProviderSessionIdTooLong =
            new(ErrorCodes.PaymentAttempt.ProviderSessionIdTooLong,
                "Provider session ID exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error ProviderPaymentIdRequired =
            new(ErrorCodes.PaymentAttempt.ProviderPaymentIdRequired,
                "Provider payment ID is required.",
                ErrorType.Validation);

        public static readonly Error ProviderPaymentIdTooLong =
            new(ErrorCodes.PaymentAttempt.ProviderPaymentIdTooLong,
                "Provider payment ID exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error PaymentUrlRequired =
            new(ErrorCodes.PaymentAttempt.PaymentUrlRequired,
                "Payment URL is required.",
                ErrorType.Validation);

        public static readonly Error PaymentUrlTooLong =
            new(ErrorCodes.PaymentAttempt.PaymentUrlTooLong,
                "Payment URL exceeds the allowed length.",
                ErrorType.Validation);

        public static readonly Error NotExpiredYet =
            new(ErrorCodes.PaymentAttempt.NotExpiredYet,
                "Payment attempt has not expired yet.",
                ErrorType.Conflict);

        public static readonly Error StatusFinal =
            new(ErrorCodes.PaymentAttempt.StatusFinal,
                "Payment attempt is already in a final state.",
                ErrorType.Conflict);

        public static readonly Error NowRequired =
            new(ErrorCodes.PaymentAttempt.NowRequired,
                "Current date/time is required.",
                ErrorType.Validation);
    }
}
