namespace E_Commerce.Domain.Common.Errors;

public static class UserAddressErrors
{
    public static readonly Error NotFound = new(ErrorCodes.UserAddress.NotFound, "User address was not found.", ErrorType.NotFound);
    public static readonly Error UserIdRequired = new(ErrorCodes.UserAddress.UserIdRequired, "User id is required.", ErrorType.Validation);
    public static readonly Error LabelInvalid = new(ErrorCodes.UserAddress.LabelInvalid, "Address label is invalid.", ErrorType.Validation);
    public static readonly Error CountryRequired = new(ErrorCodes.UserAddress.CountryRequired, "Country is required.", ErrorType.Validation);
    public static readonly Error GovernorateRequired = new(ErrorCodes.UserAddress.GovernorateRequired, "Governorate is required.", ErrorType.Validation);
    public static readonly Error CityRequired = new(ErrorCodes.UserAddress.CityRequired, "City is required.", ErrorType.Validation);
    public static readonly Error AreaRequired = new(ErrorCodes.UserAddress.AreaRequired, "Area is required.", ErrorType.Validation);
    public static readonly Error StreetRequired = new(ErrorCodes.UserAddress.StreetRequired, "Street is required.", ErrorType.Validation);
    public static readonly Error BuildingNumberRequired = new(ErrorCodes.UserAddress.BuildingNumberRequired, "Building number is required.", ErrorType.Validation);
    public static readonly Error TextTooLong = new(ErrorCodes.UserAddress.TextTooLong, "Address field is too long.", ErrorType.Validation);
    public static readonly Error LatitudeInvalid = new(ErrorCodes.UserAddress.LatitudeInvalid, "Latitude must be between -90 and 90.", ErrorType.Validation);
    public static readonly Error LongitudeInvalid = new(ErrorCodes.UserAddress.LongitudeInvalid, "Longitude must be between -180 and 180.", ErrorType.Validation);
}
