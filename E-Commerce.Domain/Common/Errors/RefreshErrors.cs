namespace E_Commerce.Domain.Common.Errors;

public static class RefreshErrors
{
    public static readonly Error DeviceInfoTooLong = new(ErrorCodes.Domain.Refresh.DeviceInfoTooLong, "Device info is too long.", ErrorType.Validation);
    public static readonly Error ExpiresAtInvalid = new(ErrorCodes.Domain.Refresh.ExpiresAtInvalid, "Refresh token expiration date is invalid.", ErrorType.Validation);
    public static readonly Error IpInvalid = new(ErrorCodes.Domain.Refresh.IpInvalid, "IP address is invalid.", ErrorType.Validation);
    public static readonly Error IpTooLong = new(ErrorCodes.Domain.Refresh.IpTooLong, "IP address is too long.", ErrorType.Validation);
    public static readonly Error NowRequired = new(ErrorCodes.Domain.Refresh.NowRequired, "Current date/time is required.", ErrorType.Validation);
    public static readonly Error ReplacedByRequired = new(ErrorCodes.Domain.Refresh.ReplacedByRequired, "Replacement token is required.", ErrorType.Validation);
    public static readonly Error ReplacedBySame = new(ErrorCodes.Domain.Refresh.ReplacedBySame, "Replacement token cannot be the same token.", ErrorType.Conflict);
    public static readonly Error TokenHashRequired = new(ErrorCodes.Domain.Refresh.TokenHashRequired, "Token hash is required.", ErrorType.Validation);
    public static readonly Error UserIdRequired = new(ErrorCodes.Domain.Refresh.UserIdRequired, "User ID is required.", ErrorType.Validation);
}
