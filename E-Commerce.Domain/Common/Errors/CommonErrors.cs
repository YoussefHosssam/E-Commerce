namespace E_Commerce.Domain.Common.Errors;

public static class CommonErrors
{
    public static readonly Error PageInvalid = new(ErrorCodes.Common.PageInvalid, "Page number must be greater than zero.", ErrorType.Validation);
    public static readonly Error PageSizeInvalid = new(ErrorCodes.Common.PageSizeInvalid, "Page size must be greater than zero.", ErrorType.Validation);

    public static Error Validation(string message = "The request is invalid.")
        => new(ErrorCodes.Common.Validation, message, ErrorType.Validation);

    public static Error NotFound(string message = "The requested resource was not found.")
        => new(ErrorCodes.Common.NotFound, message, ErrorType.NotFound);

    public static Error Conflict(string message = "The request conflicts with the current state.")
        => new(ErrorCodes.Common.Conflict, message, ErrorType.Conflict);

    public static Error Unexpected(string message = "An unexpected error occurred.")
        => new(ErrorCodes.Common.Unexpected, message, ErrorType.Failure);
}
