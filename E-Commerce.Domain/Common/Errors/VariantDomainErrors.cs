namespace E_Commerce.Domain.Common.Errors;

public static class VariantDomainErrors
{
    public static readonly Error ColorTooLong = new(ErrorCodes.Domain.Variant.ColorTooLong, "Variant color is too long.", ErrorType.Validation);
    public static readonly Error ImageRequired = new(ErrorCodes.Domain.Variant.ImageRequired, "Variant image is required.", ErrorType.Validation);
    public static readonly Error InventoryRequired = new(ErrorCodes.Domain.Variant.InventoryRequired, "Variant inventory is required.", ErrorType.Validation);
    public static readonly Error SizeTooLong = new(ErrorCodes.Domain.Variant.SizeTooLong, "Variant size is too long.", ErrorType.Validation);
}
