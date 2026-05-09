using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Payment.Paymob
{
    public class PaymobClient
    {
        private readonly HttpClient _httpClient;
        public PaymobClient(HttpClient client)
        {
            _httpClient = client;
        }
    }
}
