namespace E_Commerce.Domain.Common.Errors;

public static class ValueObjectErrors
{
    public static readonly Error CurrencyRequired =
        new(ErrorCodes.ValueObjects.CurrencyRequired, "Currency is required.", ErrorType.Validation);

    public static readonly Error CurrencyInvalid =
        new(ErrorCodes.ValueObjects.CurrencyInvalid, "Currency code must be a valid 3-letter ISO code.", ErrorType.Validation);

    public static readonly Error EmailRequired =
        new(ErrorCodes.ValueObjects.EmailRequired, "Email is required.", ErrorType.Validation);

    public static readonly Error EmailInvalid =
        new(ErrorCodes.ValueObjects.EmailInvalid, "Email format is invalid.", ErrorType.Validation);

    public static readonly Error JsonInvalid =
        new(ErrorCodes.ValueObjects.JsonInvalid, "JSON text is invalid.", ErrorType.Validation);

    public static readonly Error MoneyAmountInvalid =
        new(ErrorCodes.ValueObjects.MoneyAmountInvalid, "Money amount cannot be negative.", ErrorType.Validation);

    public static readonly Error MoneyCurrencyMismatch =
        new(ErrorCodes.ValueObjects.MoneyCurrencyMismatch, "Cannot operate on money values with different currencies.", ErrorType.Validation);

    public static readonly Error PasswordHashInvalid =
        new(ErrorCodes.ValueObjects.PasswordHashInvalid, "Password hash is invalid.", ErrorType.Validation);

    public static readonly Error SlugRequired =
        new(ErrorCodes.ValueObjects.SlugRequired, "Slug is required.", ErrorType.Validation);

    public static readonly Error SlugInvalid =
        new(ErrorCodes.ValueObjects.SlugInvalid, "Slug format is invalid.", ErrorType.Validation);

    public static readonly Error TokenHashRequired =
        new(ErrorCodes.ValueObjects.TokenHashRequired, "Token hash is required.", ErrorType.Validation);

    public static readonly Error TokenHashInvalid =
        new(ErrorCodes.ValueObjects.TokenHashInvalid, "Token hash is invalid.", ErrorType.Validation);
}