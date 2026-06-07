using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Shipment.Bosta
{
    internal class BostaClient
    {
        private HttpClient _client;
        public BostaClient(HttpClient Client)
        {
            _client = Client;
        }
    }
}
