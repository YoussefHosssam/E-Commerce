using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Features.Me.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;

namespace E_Commerce.Application.Features.Me.Queries.GetUserAddresses;

public sealed class GetUserAddressesHandler : IRequestHandler<GetUserAddressesQuery, Result<IReadOnlyCollection<UserAddressDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public GetUserAddressesHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<IReadOnlyCollection<UserAddressDto>>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
            return Result<IReadOnlyCollection<UserAddressDto>>.Fail(AuthErrors.InvalidToken);

        var user = await _uow.Users.GetByIdWithLoadingDataAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result<IReadOnlyCollection<UserAddressDto>>.Fail(UserErrors.NotFound);

        var addresses = await _uow.UserAddresses.GetByUserIdAsync(userId.Value, false, cancellationToken);
        return Result<IReadOnlyCollection<UserAddressDto>>.Success(addresses.Select(Map).ToList());
    }

    private static UserAddressDto Map(UserAddress address)
        => new(
            address.Id,
            address.Label,
            address.Country,
            address.Governorate,
            address.City,
            address.Area,
            address.Street,
            address.BuildingNumber,
            address.Floor,
            address.Apartment,
            address.PostalCode,
            address.Landmark,
            address.Latitude,
            address.Longitude,
            address.IsDefault);
}
