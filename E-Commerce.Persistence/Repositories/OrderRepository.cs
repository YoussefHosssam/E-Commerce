using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Persistence.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly DbSet<Order> _orders;

    public OrderRepository(EcommerceContext ctx) : base(ctx)
    {
        _orders = ctx.Orders;
    }

    public async Task<OrderDetailsReadModel?> GetOrderDetailsDtoAsync(Guid id, CancellationToken ctn)
    {
        var order = await _orders
            .AsNoTracking()
            .Where(o => o.Id == id && o.Status != OrderStatus.Cancelled)
            .Select(o => new OrderDetailsProjection(
                o.UserId,
                o.Id,
                o.OrderNumber,
                o.Status,
                o.Subtotal,
                o.ShippingFee,
                o.DiscountTotal,
                o.TaxTotal,
                o.GrandTotal,
                o.Currency.Value,
                o.ShippingAddressJson,
                o.BillingAddressJson,
                o.Notes,
                o.UpdatedAt,
                o.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        Sku = i.Sku,
                        ProductTitle = i.ProductTitleSnapshot,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity,
                        LineTotal = i.LineTotal,
                        Currency = i.Currency.Value
                    })
                    .ToList()))
            .FirstOrDefaultAsync(ctn);

        return order?.ToReadModel();
    }

    public async Task<PagedResult<OrderListDto>> GetOrderListItemDtosAsync(Guid userId, PageRequest page, CancellationToken ctn)
    {
        return await _orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.UpdatedAt)
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status,
                GrandTotal = o.GrandTotal,
                Currency = o.Currency.Value,
                UpdatedAt = o.UpdatedAt
            })
            .ToPagedResultAsync(page, ctn);
    }

    public async Task<Order?> GetTrackingOrderByIdWithDetailsAsync(Guid id, CancellationToken ctn)
    {
        var order = await _orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Variant)
            .FirstOrDefaultAsync(o => o.Id == id && o.Status != OrderStatus.Cancelled, ctn);

        return order;
    }

    private sealed record OrderDetailsProjection(
        Guid UserId,
        Guid Id,
        string OrderNumber,
        OrderStatus Status,
        decimal Subtotal,
        decimal ShippingFee,
        decimal DiscountTotal,
        decimal TaxTotal,
        decimal GrandTotal,
        string Currency,
        JsonText ShippingAddressJson,
        JsonText BillingAddressJson,
        string? Notes,
        DateTimeOffset? UpdatedAt,
        List<OrderItemDto> Items)
    {
        public OrderDetailsReadModel ToReadModel()
        {
            return new OrderDetailsReadModel(
                UserId,
                new OrderDto
                {
                    Id = Id,
                    OrderNumber = OrderNumber,
                    Status = Status,
                    Subtotal = Subtotal,
                    ShippingFee = ShippingFee,
                    DiscountTotal = DiscountTotal,
                    TaxTotal = TaxTotal,
                    GrandTotal = GrandTotal,
                    Currency = Currency,
                    ShippingAddress = JsonText.To<ShippingAddressDto>(ShippingAddressJson),
                    BillingAddress = JsonText.To<BillingAddressDto>(BillingAddressJson),
                    Notes = Notes,
                    UpdatedAt = UpdatedAt,
                    Items = Items
                });
        }
    }
}
