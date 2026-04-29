using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    public sealed class JwtOptions
    {
        public string Issuer { get; init; } = default!;
        public string Audience { get; init; } = default!;
        public string SigningKey { get; init; } = default!;
        public int AccessTokenMinutes { get; init; } = 15;
    }
}
