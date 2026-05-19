using E_Commerce.Domain.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Enums;

namespace E_Commerce.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;
    public AddressLabel Label { get; private set; }
    public string Country { get; private set; } = default!;
    public string Governorate { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string Area { get; private set; } = default!;
    public string Street { get; private set; } = default!;
    public string BuildingNumber { get; private set; } = default!;
    public string? Floor { get; private set; }
    public string? Apartment { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Landmark { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public bool IsDefault { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    private UserAddress() { }

    private UserAddress(Guid userId, AddressLabel label)
    {
        UserId = userId;
        Label = label;
    }

    public static UserAddress Create(
        Guid userId,
        AddressLabel label,
        string country,
        string governorate,
        string city,
        string area,
        string street,
        string buildingNumber,
        string? floor,
        string? apartment,
        string? postalCode,
        string? landmark,
        decimal? latitude,
        decimal? longitude,
        bool isDefault)
    {
        if (userId == Guid.Empty)
            throw new DomainValidationException(UserAddressErrors.UserIdRequired);

        ValidateLabel(label);

        var address = new UserAddress(userId, label);
        address.Apply(country, governorate, city, area, street, buildingNumber, floor, apartment, postalCode, landmark, latitude, longitude);
        address.IsDefault = isDefault;
        return address;
    }

    public void Update(
        AddressLabel label,
        string country,
        string governorate,
        string city,
        string area,
        string street,
        string buildingNumber,
        string? floor,
        string? apartment,
        string? postalCode,
        string? landmark,
        decimal? latitude,
        decimal? longitude,
        DateTimeOffset now)
    {
        ValidateLabel(label);
        Label = label;
        Apply(country, governorate, city, area, street, buildingNumber, floor, apartment, postalCode, landmark, latitude, longitude);
        UpdatedAt = now;
    }

    public void SetDefault(bool isDefault, DateTimeOffset now)
    {
        IsDefault = isDefault;
        UpdatedAt = now;
    }

    private void Apply(
        string country,
        string governorate,
        string city,
        string area,
        string street,
        string buildingNumber,
        string? floor,
        string? apartment,
        string? postalCode,
        string? landmark,
        decimal? latitude,
        decimal? longitude)
    {
        Country = NormalizeRequired(country, UserAddressErrors.CountryRequired, 100);
        Governorate = NormalizeRequired(governorate, UserAddressErrors.GovernorateRequired, 100);
        City = NormalizeRequired(city, UserAddressErrors.CityRequired, 100);
        Area = NormalizeRequired(area, UserAddressErrors.AreaRequired, 150);
        Street = NormalizeRequired(street, UserAddressErrors.StreetRequired, 200);
        BuildingNumber = NormalizeRequired(buildingNumber, UserAddressErrors.BuildingNumberRequired, 50);
        Floor = NormalizeOptional(floor, 50);
        Apartment = NormalizeOptional(apartment, 50);
        PostalCode = NormalizeOptional(postalCode, 30);
        Landmark = NormalizeOptional(landmark, 300);

        if (latitude is < -90 or > 90)
            throw new DomainValidationException(UserAddressErrors.LatitudeInvalid);

        if (longitude is < -180 or > 180)
            throw new DomainValidationException(UserAddressErrors.LongitudeInvalid);

        Latitude = latitude;
        Longitude = longitude;
    }

    private static void ValidateLabel(AddressLabel label)
    {
        if (!Enum.IsDefined(typeof(AddressLabel), label))
            throw new DomainValidationException(UserAddressErrors.LabelInvalid);
    }

    private static string NormalizeRequired(string value, Error requiredError, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException(requiredError);

        value = value.Trim();

        if (value.Length > maxLength)
            throw new DomainValidationException(UserAddressErrors.TextTooLong);

        return value;
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (value is null)
            return null;

        value = value.Trim();
        if (value.Length == 0)
            return null;

        if (value.Length > maxLength)
            throw new DomainValidationException(UserAddressErrors.TextTooLong);

        return value;
    }
}
