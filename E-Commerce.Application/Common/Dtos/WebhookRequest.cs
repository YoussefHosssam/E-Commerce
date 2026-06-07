using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Dtos
{
    public record WebhookRequest(string rawBody , IDictionary<string , string> headers , IDictionary<string, string> queries);
}
