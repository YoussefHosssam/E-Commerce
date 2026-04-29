namespace E_Commerce.Domain.Common.Errors;

public static class VariantImageErrors
{
    public static readonly Error SortOrderInvalid = new(ErrorCodes.Domain.VariantImage.SortOrderInvalid, "Variant image sort order is invalid.", ErrorType.Validation);
    public static readonly Error UrlEmpty = new(ErrorCodes.Domain.VariantImage.UrlEmpty, "Variant image URL is empty.", ErrorType.Validation);
    public static readonly Error VariantIdEmpty = new(ErrorCodes.Domain.VariantImage.VariantIdEmpty, "Variant image variant ID is empty.", ErrorType.Validation);
}
