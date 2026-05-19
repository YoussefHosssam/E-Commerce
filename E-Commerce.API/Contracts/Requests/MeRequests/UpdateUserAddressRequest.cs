using E_Commerce.Domain.Enums;

namespace E_Commerce.API.Contracts.Requests.MeRequests;

public sealed record UpdateUserAddressRequest(
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
    decimal? Longitude);
