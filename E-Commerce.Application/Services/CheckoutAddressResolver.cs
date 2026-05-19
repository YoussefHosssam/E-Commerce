using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Services;

public sealed class CheckoutAddressResolver : ICheckoutAddressResolver
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public CheckoutAddressResolver(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<ResolvedCheckoutAddress>> ResolveAsync(CheckoutAddressSelection selection, CancellationToken ct)
    {
        if (!_userAccessor.UserId.HasValue)
            return Result<ResolvedCheckoutAddress>.Fail(AuthErrors.InvalidToken);

        var user = await _uow.Users.GetByIdWithLoadingDataAsync(_userAccessor.UserId.Value, ct);
        if (user is null)
            return Result<ResolvedCheckoutAddress>.Fail(UserErrors.NotFound);

        UserAddress? address;

        if (selection.DefaultAddress)
        {
            var addresses = await _uow.UserAddresses.GetByUserIdAsync(user.Id, false, ct);
            address = addresses.FirstOrDefault(x => x.IsDefault);

            if (address is null)
                return Result<ResolvedCheckoutAddress>.Fail(CheckoutErrors.DefaultAddressNotFound);
        }
        else
        {
            if (!selection.AddressId.HasValue || selection.AddressId.Value == Guid.Empty)
                return Result<ResolvedCheckoutAddress>.Fail(CheckoutErrors.AddressRequired);

            address = await _uow.UserAddresses.GetByIdAsync(selection.AddressId.Value, false, ct);

            if (address is null)
                return Result<ResolvedCheckoutAddress>.Fail(UserAddressErrors.NotFound);

            if (address.UserId != user.Id)
                return Result<ResolvedCheckoutAddress>.Fail(CheckoutErrors.AddressDoesNotBelongToUser);
        }

        if (!IsUsableForShipping(address) || string.IsNullOrWhiteSpace(user.Phone))
            return Result<ResolvedCheckoutAddress>.Fail(CheckoutErrors.AddressInvalidForShipping);

        return Result<ResolvedCheckoutAddress>.Success(
            new ResolvedCheckoutAddress(user, address, ToShippingAddress(user, address)));
    }

    private static bool IsUsableForShipping(UserAddress address)
        => !string.IsNullOrWhiteSpace(address.Country)
           && !string.IsNullOrWhiteSpace(address.Governorate)
           && !string.IsNullOrWhiteSpace(address.City)
           && !string.IsNullOrWhiteSpace(address.Area)
           && !string.IsNullOrWhiteSpace(address.Street)
           && !string.IsNullOrWhiteSpace(address.BuildingNumber);

    private static ShippingAddressDto ToShippingAddress(User user, UserAddress address)
        => new(
            user.FirstName,
            user.LastName,
            user.Email.Value,
            user.Phone!,
            address.City,
            $"{address.Street}, {address.BuildingNumber}, {address.Area}, {address.Governorate}, {address.Country}",
            BuildAddressLine2(address));

    private static string? BuildAddressLine2(UserAddress address)
    {
        var parts = new[]
        {
            string.IsNullOrWhiteSpace(address.Floor) ? null : $"Floor {address.Floor}",
            string.IsNullOrWhiteSpace(address.Apartment) ? null : $"Apartment {address.Apartment}",
            string.IsNullOrWhiteSpace(address.PostalCode) ? null : $"PostalCode {address.PostalCode}",
            string.IsNullOrWhiteSpace(address.Landmark) ? null : $"Landmark {address.Landmark}"
        }.Where(x => x is not null);

        var line = string.Join(", ", parts);
        return string.IsNullOrWhiteSpace(line) ? null : line;
    }
}
