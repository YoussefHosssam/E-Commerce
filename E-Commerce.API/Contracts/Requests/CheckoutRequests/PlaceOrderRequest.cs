using E_Commerce.Application.Common.Dtos;

namespace E_Commerce.API.Contracts.Requests.CheckoutRequests
{
    public record PlaceOrderRequest(ShippingAddressDto ShippingAddress , bool SameAsShipping , BillingAddressDto? BillingAddress);
}
