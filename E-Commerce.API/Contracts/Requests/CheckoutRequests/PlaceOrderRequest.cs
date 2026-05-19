using E_Commerce.Application.Common.Dtos;

namespace E_Commerce.API.Contracts.Requests.CheckoutRequests
{
    public record PlaceOrderRequest(bool DefaultAddress , Guid AddressId , bool SameAsShipping , BillingAddressDto? BillingAddress);
}
