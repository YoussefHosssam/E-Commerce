using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.UpdateUserAddress;

public sealed class UpdateUserAddressHandler : IRequestHandler<UpdateUserAddressCommand, Result<UserAddressDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public UpdateUserAddressHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<UserAddressDto>> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
            return Result<UserAddressDto>.Fail(AuthErrors.InvalidToken);

        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result<UserAddressDto>.Fail(UserErrors.NotFound);

        var address = await _uow.UserAddresses.GetByIdForUserAsync(userId.Value, request.AddressId, true, cancellationToken);
        if (address is null)
            return Result<UserAddressDto>.Fail(UserAddressErrors.NotFound);

        address.Update(
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
            DateTimeOffset.UtcNow);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result<UserAddressDto>.Success(Map(address));
    }

    private static UserAddressDto Map(UserAddress address)
        => new(address.Id, address.Label, address.Country, address.Governorate, address.City, address.Area, address.Street,
            address.BuildingNumber, address.Floor, address.Apartment, address.PostalCode, address.Landmark, address.Latitude,
            address.Longitude, address.IsDefault);
}
