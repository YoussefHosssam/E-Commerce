using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.MeRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Features.Me.Commands.CreateUserAddress;
using E_Commerce.Application.Features.Me.Commands.DeleteUserAddress;
using E_Commerce.Application.Features.Me.Commands.SetDefaultUserAddress;
using E_Commerce.Application.Features.Me.Commands.UpdateUserAddress;
using E_Commerce.Application.Features.Me.Commands.UpdateUserProfile;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Application.Features.Me.Queries.GetCurrentUser;
using E_Commerce.Application.Features.Me.Queries.GetUserAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Authorize]
[Route("api/v{version:apiVersion}/me")]
public sealed class MeController : ControllerBase
{
    private readonly ISender _sender;

    public MeController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<UserResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<UserResponse>> GetMe(CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new GetCurrentUserQuery(), ct),
            user => new UserResponse(user),
            "Current user retrieved successfully.");

    [HttpPatch("profile")]
    [ProducesResponseType(typeof(ApiResult<UserProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<UserProfileResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<UserProfileResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<UserProfileResponse>> UpdateProfile(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken ct)
    {
        var command = new UpdateUserProfileCommand(
            request.FirstName,
            request.LastName,
            request.DisplayName,
            request.Gender,
            request.DateOfBirth,
            request.AvatarUrl);

        return this.FromResult(
            await _sender.Send(command, ct),
            userProfile => new UserProfileResponse(userProfile),
            "Profile updated successfully.");
    }

    [HttpGet("addresses")]
    [ProducesResponseType(typeof(ApiResult<AddressesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AddressesResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<AddressesResponse>> GetAddresses(CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new GetUserAddressesQuery(), ct),
            addresses => new AddressesResponse(addresses),
            "Addresses retrieved successfully.");

    [HttpPost("addresses")]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<AddressResponse>> CreateAddress(
        [FromBody] CreateUserAddressRequest request,
        CancellationToken ct)
    {
        var command = new CreateUserAddressCommand(
            request.Label,
            request.Country,
            request.Governorate,
            request.City,
            request.Area,
            request.Street,
            request.BuildingNumber,
            request.Floor,
            request.Apartment,
            request.PostalCode,
            request.Landmark,
            request.Latitude,
            request.Longitude,
            request.IsDefault);

        return this.FromResult(
            await _sender.Send(command, ct),
            address => new AddressResponse(address),
            "Address created successfully.",
            StatusCodes.Status201Created);
    }

    [HttpPatch("addresses/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<AddressResponse>> UpdateAddress(
        Guid id,
        [FromBody] UpdateUserAddressRequest request,
        CancellationToken ct)
    {
        var command = new UpdateUserAddressCommand(
            id,
            request.Label,
            request.Country,
            request.Governorate,
            request.City,
            request.Area,
            request.Street,
            request.BuildingNumber,
            request.Floor,
            request.Apartment,
            request.PostalCode,
            request.Landmark,
            request.Latitude,
            request.Longitude);

        return this.FromResult(
            await _sender.Send(command, ct),
            address => new AddressResponse(address),
            "Address updated successfully.");
    }

    [HttpDelete("addresses/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> DeleteAddress(Guid id, CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new DeleteUserAddressCommand(id), ct),
            "Address deleted successfully.");

    [HttpPatch("addresses/{id:guid}/default")]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<AddressResponse>), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<AddressResponse>> SetDefaultAddress(Guid id, CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new SetDefaultUserAddressCommand(id), ct),
            address => new AddressResponse(address),
            "Default address updated successfully.");
}
