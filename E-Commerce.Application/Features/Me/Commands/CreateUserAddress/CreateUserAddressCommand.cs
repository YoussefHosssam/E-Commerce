using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Enums;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.CreateUserAddress;

public sealed record CreateUserAddressCommand(
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
    bool IsDefault) : IRequest<Result<UserAddressDto>>;
