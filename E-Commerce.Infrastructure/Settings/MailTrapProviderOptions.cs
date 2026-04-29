using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    internal class MailTrapProviderOptions
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; }
        public string Username { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string Sender { get; init; } = default!;
    }
}
