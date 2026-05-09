using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Settings
{
    internal class PaymobPyamentOptions
    {
        public string PublicKey { get; init; } = default!;
        public string SecretKey { get; init; } = default!;
        public string HMAC { get; init; } = default!;
        public required IReadOnlyCollection<int> PaymentMethods { get; init; }
    }


}
