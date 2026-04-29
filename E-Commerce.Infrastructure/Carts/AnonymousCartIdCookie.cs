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
            return req.Cookies.TryGetValue(_opt.CookieName, out var v) ? v : null;
        }

        public void Write(string value)
        {
            var res = _ctx.HttpContext?.Response;
            if (res is null) return;

            res.Cookies.Append(_opt.CookieName, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = _opt.Secure,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(_opt.DaysToExpire)
            });
        }

        public void Delete()
        {
            var res = _ctx.HttpContext?.Response;
            if (res is null) return;
            res.Cookies.Delete(_opt.CookieName);
        }
    }
}
