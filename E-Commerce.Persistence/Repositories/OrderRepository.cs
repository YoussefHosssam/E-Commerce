using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order> , IOrderRepository
    {
        private readonly DbSet<Order> _orders;
        public OrderRepository(EcommerceContext ctx) : base(ctx)
        {
            _orders = ctx.Orders;
        }
    }
}
