using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    public sealed class RefreshTokenOptions
    {
        public int DaysToExpire { get; init; } = 30;
        public int TokenBytes { get; init; } = 64; // random bytes
    }
}
