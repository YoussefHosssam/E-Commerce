using AutoMapper;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using CartEntity = E_Commerce.Domain.Entities.Cart;

namespace E_Commerce.Application.Features.Checkout.Queries;

public sealed class ReviewCheckoutHandler : IRequestHandler<ReviewCheckoutQuery, Result<CheckoutReviewDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICheckoutAddressResolver _addressResolver;
    private readonly IShipmentFeesCalculator _shipmentFeesCalculator;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewCheckoutHandler> _logger;

    public ReviewCheckoutHandler(
        IUnitOfWork uow,
        ICheckoutAddressResolver addressResolver,
        IShipmentFeesCalculator shipmentFeesCalculator,
        IMapper mapper,
        ILogger<ReviewCheckoutHandler> logger)
    {
        _uow = uow;
        _addressResolver = addressResolver;
        _shipmentFeesCalculator = shipmentFeesCalculator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CheckoutReviewDto>> Handle(ReviewCheckoutQuery request, CancellationToken cancellationToken)
    {
        var addressResult = await _addressResolver.ResolveAsync(
            new CheckoutAddressSelection(request.DefaultAddress, request.AddressId),
            cancellationToken);

        if (!addressResult.IsSuccess)
            return Result<CheckoutReviewDto>.Fail(addressResult.Error!);

        var resolvedAddress = addressResult.Data!;
        var cart = await _uow.Carts.GetCartWithItemsByUserId(resolvedAddress.User.Id, cancellationToken);

        if (cart is null || !cart.Items.Any())
            return Result<CheckoutReviewDto>.Fail(CheckoutErrors.EmptyCart);

        var shipmentFeeResult = await CalculateShipmentFeeAsync(
            resolvedAddress,
            request.SameAsShipping,
            request.BillingAddress);

        if (!shipmentFeeResult.IsSuccess)
            return Result<CheckoutReviewDto>.Fail(shipmentFeeResult.Error!);

        var summary = _mapper.Map<CheckoutSummaryDto>(cart);
        var shippingFee = shipmentFeeResult.Data;

        return Result<CheckoutReviewDto>.Success(
            new CheckoutReviewDto(
                summary.Items,
                summary.TotalItems,
                summary.TotalQuantity,
                summary.Subtotal,
                shippingFee,
                summary.Subtotal + shippingFee,
                summary.Currency,
                MapAddress(resolvedAddress.Address)));
    }

    private async Task<Result<decimal>> CalculateShipmentFeeAsync(
        ResolvedCheckoutAddress resolvedAddress,
        bool sameAsShipping,
        E_Commerce.Application.Common.Dtos.BillingAddressDto? billingAddress)
    {
        try
        {
            var result = await _shipmentFeesCalculator.CalculateFees(
                resolvedAddress.ShippingAddress,
                sameAsShipping,
                billingAddress);

            if (result is null || !result.IsSuccess || result.Data < 0)
                return Result<decimal>.Fail(CheckoutErrors.ShipmentFeeCalculationFailed);

            return Result<decimal>.Success(result.Data);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Shipment fee calculation failed for User {UserId} Address {AddressId}",
                resolvedAddress.User.Id,
                resolvedAddress.Address.Id);

            return Result<decimal>.Fail(CheckoutErrors.ShipmentFeeCalculationFailed);
        }
    }

    private static CheckoutAddressDto MapAddress(UserAddress address)
        => new(
            address.Id,
            address.Label.ToString(),
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
