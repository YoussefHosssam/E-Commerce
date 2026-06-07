using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Checkout.Common;
using MediatR;

namespace E_Commerce.Application.Features.Checkout.Queries;

public sealed record ReviewCheckoutQuery(
    bool DefaultAddress,
    Guid? AddressId,
    bool SameAsShipping,
    BillingAddressDto? BillingAddress) : IRequest<Result<CheckoutReviewDto>>;
