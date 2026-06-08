using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Features.Order.Common;
using MediatR;

namespace E_Commerce.Application.Features.Order.Queries;

public record GetOrdersQuery(PageRequest page) : IRequest<Result<List<OrderListDto>>>;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<List<OrderListDto>>>
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUnitOfWork _uow;

    public GetOrdersHandler(IUserAccessor userAccessor, IUnitOfWork uow)
    {
        _userAccessor = userAccessor;
        _uow = uow;
    }

    public async Task<Result<List<OrderListDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = _userAccessor.GetRequiredUserId();
        var pagedOrders = await _uow.Orders.GetOrderListItemDtosAsync(userId, request.page, cancellationToken);
        return Result<List<OrderListDto>>.Success(pagedOrders.Items.ToList(), pagedOrders.ToMetaResult());
    }
}
