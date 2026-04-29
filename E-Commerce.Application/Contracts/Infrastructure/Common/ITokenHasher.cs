using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Common
{
    public interface ITokenHasher
    {
        public Task<TokenHash> HashAsync(string token, CancellationToken ctn);
    }
}
