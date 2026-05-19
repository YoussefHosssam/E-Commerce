using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.MeRequests;
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
    [ProducesResponseType(typeof(ApiResult<CurrentUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<CurrentUserDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<CurrentUserDto>> GetMe(CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new GetCurrentUserQuery(), ct),
            "Current user retrieved successfully.");

    [HttpPatch("profile")]
    [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<UserProfileDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<UserProfileDto>> UpdateProfile(
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
            "Profile updated successfully.");
    }

    [HttpGet("addresses")]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyCollection<UserAddressDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<IReadOnlyCollection<UserAddressDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<IReadOnlyCollection<UserAddressDto>>> GetAddresses(CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new GetUserAddressesQuery(), ct),
            "Addresses retrieved successfully.");

    [HttpPost("addresses")]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<UserAddressDto>> CreateAddress(
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
            "Address created successfully.",
            StatusCodes.Status201Created);
    }

    [HttpPatch("addresses/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<UserAddressDto>> UpdateAddress(
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
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult<UserAddressDto>), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<UserAddressDto>> SetDefaultAddress(Guid id, CancellationToken ct)
        => this.FromResult(
            await _sender.Send(new SetDefaultUserAddressCommand(id), ct),
            "Default address updated successfully.");
}
