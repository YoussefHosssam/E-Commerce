using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    internal class RedisOptions
    {
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string Host { get; init; } = default!;
        public int Port { get; init; } = default!;

    }
}
