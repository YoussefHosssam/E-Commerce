using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.SetDefaultUserAddress;

public sealed class SetDefaultUserAddressHandler : IRequestHandler<SetDefaultUserAddressCommand, Result<UserAddressDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public SetDefaultUserAddressHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<UserAddressDto>> Handle(SetDefaultUserAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId, cancellationToken);
        if (user is null)
            return Result<UserAddressDto>.Fail(UserErrors.NotFound);

        var addresses = await _uow.UserAddresses.GetByUserIdAsync(userId, true, cancellationToken);
        var target = addresses.FirstOrDefault(x => x.Id == request.AddressId);
        if (target is null)
            return Result<UserAddressDto>.Fail(UserAddressErrors.NotFound);

        var now = DateTimeOffset.UtcNow;
        foreach (var address in addresses)
        {
            address.SetDefault(address.Id == target.Id, now);
        }

        await _uow.SaveChangesAsync(cancellationToken);
        return Result<UserAddressDto>.Success(Map(target));
    }

    private static UserAddressDto Map(UserAddress address)
        => new(address.Id, address.Label, address.Country, address.Governorate, address.City, address.Area, address.Street,
            address.BuildingNumber, address.Floor, address.Apartment, address.PostalCode, address.Landmark, address.Latitude,
            address.Longitude, address.IsDefault);
}
