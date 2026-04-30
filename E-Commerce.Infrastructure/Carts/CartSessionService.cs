using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Carts
{
    public sealed class CartSessionService : ICartSessionService
    {
        private readonly IAnonymousCartIdCookie _cookie;

        public CartSessionService(IAnonymousCartIdCookie cookie) => _cookie = cookie;

        public string? GetAnonymousId() => _cookie.Read();

        public string CreateAnonymousId()
        {
            var id = Guid.NewGuid().ToString("N");
            return id;
        }
    }
}
