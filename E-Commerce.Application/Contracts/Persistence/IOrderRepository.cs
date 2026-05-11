using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Persistence
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        public Task<Order?> GetOrderByIdWithDetailsAsync(Guid id, CancellationToken ctn);
        public Task<PagedResult<Order>> GetOrdersWithDetailsAsync(Guid userId, PageRequest page, CancellationToken ctn);

    }
}
