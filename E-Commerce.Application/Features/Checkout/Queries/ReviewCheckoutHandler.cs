using AutoMapper;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Features.Checkout.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using CartEntity = E_Commerce.Domain.Entities.Cart;

namespace E_Commerce.Application.Features.Checkout.Queries;

internal sealed class ReviewCheckoutHandler : IRequestHandler<ReviewCheckoutQuery, Result<CheckoutReviewDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICheckoutAddressResolver _addressResolver;
    private readonly IShipmentProvider _shipmentFeesCalculator;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewCheckoutHandler> _logger;
    private readonly IShipmentFeesService _shipmentFeesService;

    public ReviewCheckoutHandler(
        IUnitOfWork uow,
        ICheckoutAddressResolver addressResolver,
        IShipmentProvider shipmentFeesCalculator,
        IMapper mapper,
        ILogger<ReviewCheckoutHandler> logger,
        IShipmentFeesService shipmentFeesService)
    {
        _uow = uow;
        _addressResolver = addressResolver;
        _shipmentFeesCalculator = shipmentFeesCalculator;
        _mapper = mapper;
        _logger = logger;
        _shipmentFeesService = shipmentFeesService;
    }

    public async Task<Result<CheckoutReviewDto>> Handle(ReviewCheckoutQuery request, CancellationToken cancellationToken)
    {
        var addressResult = await _addressResolver.ResolveAsync(
            new CheckoutAddressSelection(request.DefaultAddress, request.AddressId),
            cancellationToken);

        if (!addressResult.IsSuccess)
            return Result<CheckoutReviewDto>.Fail(addressResult.Error!);

        var resolvedAddress = addressResult.Data!;
        var summary = await _uow.Carts.GetCheckoutSummaryDtoByUserIdAsync(resolvedAddress.User.Id, cancellationToken);

        if (summary is null || !summary.Items.Any())
            return Result<CheckoutReviewDto>.Fail(CheckoutErrors.EmptyCart);

        var shipmentFeeResult = await _shipmentFeesService.CalculateShipmentFeeAsync(resolvedAddress, summary.Subtotal, cancellationToken);

        if (!shipmentFeeResult.IsSuccess)
            return Result<CheckoutReviewDto>.Fail(shipmentFeeResult.Error!);

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

    private CheckoutAddressDto MapAddress(UserAddress address)
        => _mapper.Map<CheckoutAddressDto>(address);
}
