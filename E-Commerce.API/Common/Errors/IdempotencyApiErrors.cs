using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.API.Common.Errors;

public static class IdempotencyApiErrors
{
    public static readonly Error UnsupportedMethod =
        new("IDEM_400_UNSUPPORTED_METHOD",
            "Idempotency is only supported for POST, PUT, or PATCH requests.",
            ErrorType.Validation);

    public static readonly Error HeaderRequired =
        new("IDEM_400_HEADER_REQUIRED",
            "Idempotency-Key header is required.",
            ErrorType.Validation);

    public static readonly Error HeaderEmpty =
        new("IDEM_400_HEADER_EMPTY",
            "Idempotency-Key header cannot be empty.",
            ErrorType.Validation);

    public static readonly Error HeaderTooLong =
        new("IDEM_400_HEADER_TOO_LONG",
            "Idempotency-Key header is too long.",
            ErrorType.Validation);
    
    public static readonly Error UserIdNotFound =
    new("IDEM_400_USER_ID_NOT_FOUND",
        "User id is not found.",
        ErrorType.Unauthorized);

    public static readonly Error KeyUsedWithDifferentRequest =
        new("IDEM_409_KEY_USED_WITH_DIFFERENT_REQUEST",
            "Idempotency-Key was already used with a different request body.",
            ErrorType.Conflict);

    public static readonly Error RequestAlreadyProcessing =
        new("IDEM_409_REQUEST_ALREADY_PROCESSING",
            "A request with the same Idempotency-Key is already being processed.",
            ErrorType.Conflict);

    public static readonly Error PreviousRequestFailed =
        new("IDEM_409_PREVIOUS_REQUEST_FAILED",
            "A previous request with the same Idempotency-Key failed. Use a new Idempotency-Key.",
            ErrorType.Conflict);

    public static readonly Error KeyExpired =
    new("IDEM_409_KEY_EXPIRED",
        "Idempotency-Key is invalid.",
        ErrorType.Conflict);
}