using E_Commerce.Domain.Enums;

namespace E_Commerce.Application.Features.Me.Common;

public sealed record UserProfileDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string DisplayName,
    Gender? Gender,
    DateOnly? DateOfBirth,
    string? AvatarUrl);

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string DisplayName,
    string? Phone,
    string Role,
    UserProfileDto Profile);

public sealed record UserAddressDto(
    Guid Id,
    AddressLabel Label,
    string Country,
    string Governorate,
    string City,
    string Area,
    string Street,
    string BuildingNumber,
    string? Floor,
    string? Apartment,
    string? PostalCode,
    string? Landmark,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault);
