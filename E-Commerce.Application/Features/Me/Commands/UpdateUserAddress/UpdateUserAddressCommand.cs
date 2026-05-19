using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Enums;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserAddress;

public sealed record UpdateUserAddressCommand(
    Guid AddressId,
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
    decimal? Longitude) : IRequest<Result<UserAddressDto>>;
