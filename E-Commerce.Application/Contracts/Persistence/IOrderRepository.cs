using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetTrackingOrderByIdWithDetailsAsync(Guid id, CancellationToken ctn);
    Task<OrderDetailsReadModel?> GetOrderDetailsDtoAsync(Guid id, CancellationToken ctn);
    Task<PagedResult<OrderListDto>> GetOrderListItemDtosAsync(Guid userId, PageRequest page, CancellationToken ctn);
}
