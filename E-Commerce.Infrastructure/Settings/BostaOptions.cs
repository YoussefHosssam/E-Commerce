using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    public class BostaOptions
    {
        public string Email { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public string PickUpCity { get; init; } = default!;
        public string Type { get; init; } = default!;
        public string Size { get; init; } = default!;
    }
}
