namespace E_Commerce.Domain.Common.Errors;

public static class RefreshErrors
{
    public static readonly Error DeviceInfoTooLong = new(ErrorCodes.Refresh.DeviceInfoTooLong, "Device info is too long.", ErrorType.Validation);
    public static readonly Error ExpiresAtInvalid = new(ErrorCodes.Refresh.ExpiresAtInvalid, "Refresh token expiration date is invalid.", ErrorType.Validation);
    public static readonly Error IpInvalid = new(ErrorCodes.Refresh.IpInvalid, "IP address is invalid.", ErrorType.Validation);
    public static readonly Error IpTooLong = new(ErrorCodes.Refresh.IpTooLong, "IP address is too long.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Refresh.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error ReplacedByRequired = new(ErrorCodes.Refresh.ReplacedByRequired, "Replacement token is required.", ErrorType.Validation);
    public static readonly Error ReplacedBySame = new(ErrorCodes.Refresh.ReplacedBySame, "Replacement token cannot be the same token.", ErrorType.Conflict);
    public static readonly Error TokenHashRequired = new(ErrorCodes.Refresh.TokenHashRequired, "Token hash is required.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Refresh.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
