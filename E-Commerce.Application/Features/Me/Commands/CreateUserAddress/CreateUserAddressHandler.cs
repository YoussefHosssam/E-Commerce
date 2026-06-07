using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Me.Commands.CreateUserAddress;

public sealed class CreateUserAddressHandler : IRequestHandler<CreateUserAddressCommand, Result<UserAddressDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public CreateUserAddressHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<UserAddressDto>> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId, cancellationToken);
        if (user is null)
            return Result<UserAddressDto>.Fail(UserErrors.NotFound);

        var existingAddresses = await _uow.UserAddresses.GetByUserIdAsync(user.Id, true, cancellationToken);
        var shouldBeDefault = request.IsDefault || existingAddresses.Count == 0;
        var now = DateTimeOffset.UtcNow;

        if (shouldBeDefault)
        {
            foreach (var existingAddress in existingAddresses)
            {
                existingAddress.SetDefault(false, now);
            }
        }

        var address = UserAddress.Create(
            user.Id,
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
            shouldBeDefault);

        await _uow.UserAddresses.CreateAsync(address, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<UserAddressDto>.Success(Map(address));
    }

    private static UserAddressDto Map(UserAddress address)
        => new(address.Id, address.Label, address.Country, address.Governorate, address.City, address.Area, address.Street,
            address.BuildingNumber, address.Floor, address.Apartment, address.PostalCode, address.Landmark, address.Latitude,
            address.Longitude, address.IsDefault);
}
