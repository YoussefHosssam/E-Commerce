using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Services;

public sealed record CheckoutAddressSelection(bool DefaultAddress, Guid? AddressId);

public sealed record ResolvedCheckoutAddress(User User, UserAddress Address, ShippingAddressDto ShippingAddress);

public interface ICheckoutAddressResolver
{
    Task<Result<ResolvedCheckoutAddress>> ResolveAsync(CheckoutAddressSelection selection, CancellationToken ct);
}
