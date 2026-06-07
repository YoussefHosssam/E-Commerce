namespace E_Commerce.Domain.Common.Errors;

public static class ShipmentProviderErrors
{
    public static readonly Error AuthenticationFailed =
        new(
            ErrorCodes.ShipmentProvider.AuthenticationFailed,
            "Shipment provider authentication failed.",
            ErrorType.Failure);

    public static readonly Error AccessTokenMissing =
        new(
            ErrorCodes.ShipmentProvider.AccessTokenMissing,
            "Shipment provider access token is missing.",
            ErrorType.Failure);

    public static readonly Error RefreshTokenMissing =
        new(
            ErrorCodes.ShipmentProvider.RefreshTokenMissing,
            "Shipment provider refresh token is missing.",
            ErrorType.Failure);

    public static readonly Error TokenRefreshFailed =
        new(
            ErrorCodes.ShipmentProvider.TokenRefreshFailed,
            "Shipment provider token refresh failed.",
            ErrorType.Failure);

    public static readonly Error Unauthorized =
        new(
            ErrorCodes.ShipmentProvider.Unauthorized,
            "Shipment provider request is unauthorized.",
            ErrorType.Unauthorized);

    public static readonly Error RequestFailed =
        new(
            ErrorCodes.ShipmentProvider.RequestFailed,
            "Shipment provider request failed.",
            ErrorType.Failure);

    public static readonly Error ServiceUnavailable =
        new(
            ErrorCodes.ShipmentProvider.ServiceUnavailable,
            "Shipment provider service is unavailable.",
            ErrorType.Failure);

    public static readonly Error InvalidResponse =
        new(
            ErrorCodes.ShipmentProvider.InvalidResponse,
            "Shipment provider returned an invalid response.",
            ErrorType.Failure);

    public static readonly Error ShipmentCreationFailed =
        new(
            ErrorCodes.ShipmentProvider.ShipmentCreationFailed,
            "Failed to create shipment.",
            ErrorType.Failure);

    public static readonly Error TrackingFailed =
        new(
            ErrorCodes.ShipmentProvider.TrackingFailed,
            "Failed to retrieve shipment tracking information.",
            ErrorType.Failure);
}