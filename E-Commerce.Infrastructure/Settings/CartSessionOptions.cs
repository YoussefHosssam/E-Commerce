using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    public sealed class CartSessionOptions
    {
        public string CookieName { get; init; } = "cart_id";
        public int DaysToExpire { get; init; } = 30;
        public bool Secure { get; init; } = true;
    }
}
