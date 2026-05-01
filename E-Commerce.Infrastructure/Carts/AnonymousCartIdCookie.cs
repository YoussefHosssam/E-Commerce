using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Carts
{
    public sealed class AnonymousCartIdCookie : IAnonymousCartIdCookie
    {
        private readonly IHttpContextAccessor _ctx;
        private readonly CartSessionOptions _opt;

        public AnonymousCartIdCookie(IHttpContextAccessor ctx, IOptions<CartSessionOptions> opt)
        {
            _ctx = ctx;
            _opt = opt.Value;
        }

        public string? Read()
        {
            var req = _ctx.HttpContext?.Request;
            if (req is null) return null;
            return req.Headers.TryGetValue(_opt.HeaderName, out var v) ? v.ToString() : null;
        }
    }
}
