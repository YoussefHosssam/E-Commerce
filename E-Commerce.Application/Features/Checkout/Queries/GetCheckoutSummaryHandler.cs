using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Checkout.Queries;

public class GetCheckoutSummaryHandler : IRequestHandler<GetCheckoutSummaryQuery, Result<CheckoutSummaryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public GetCheckoutSummaryHandler(IUnitOfWork uow, IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result<CheckoutSummaryDto>> Handle(GetCheckoutSummaryQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var checkoutSummary = await _uow.Carts.GetCheckoutSummaryDtoByUserIdAsync(userId, cancellationToken);
        if (checkoutSummary is null || !checkoutSummary.Items.Any())
            return Result<CheckoutSummaryDto>.Fail(CheckoutErrors.EmptyCart);

        return Result<CheckoutSummaryDto>.Success(checkoutSummary);
    }
}
