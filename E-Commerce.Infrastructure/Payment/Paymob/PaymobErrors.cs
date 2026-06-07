using E_Commerce.Domain.Common.Errors;

internal static class PaymobErrors
{
    public static readonly Error CancelRequest =
        new(
            "PAYMOB_499_CANCEL_REQUEST",
            "The Paymob request was cancelled.",
            ErrorType.Failure);

    public static readonly Error FailedRequest =
        new(
            "PAYMOB_500_FAILED_REQUEST",
            "Paymob request failed.",
            ErrorType.Failure);

    public static readonly Error MissingWebhookHmac =
        new(
            "PAYMOB_400_MISSING_WEBHOOK_HMAC",
            "Paymob webhook HMAC is missing.",
            ErrorType.Validation);

    public static readonly Error InvalidWebhookHmac =
        new(
            "PAYMOB_401_INVALID_WEBHOOK_HMAC",
            "Paymob webhook HMAC validation failed.",
            ErrorType.Unauthorized);

    public static readonly Error InvalidWebhookPayload =
        new(
            "PAYMOB_400_INVALID_WEBHOOK_PAYLOAD",
            "Paymob webhook payload is invalid.",
            ErrorType.Validation);

    public static readonly Error InvalidWebhookTransaction =
        new(
            "PAYMOB_400_INVALID_WEBHOOK_TRANSACTION",
            "Paymob webhook transaction data is invalid.",
            ErrorType.Validation);

    public static readonly Error UnsupportedWebhookEvent =
        new(
            "PAYMOB_400_UNSUPPORTED_WEBHOOK_EVENT",
            "Paymob webhook event type is not supported.",
            ErrorType.Validation);

    public static readonly Error MissingWebhookTransactionId =
        new(
            "PAYMOB_400_MISSING_WEBHOOK_TRANSACTION_ID",
            "Paymob webhook transaction id is missing.",
            ErrorType.Validation);
}