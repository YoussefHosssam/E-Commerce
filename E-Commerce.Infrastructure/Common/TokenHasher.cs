using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Common
{
    internal class TokenHasher : ITokenHasher
    {
        public async Task<TokenHash> HashAsync(string token, CancellationToken ctn)
        {
            Stream byteStream = new MemoryStream(Encoding.UTF8.GetBytes(token));
            var bytes = await SHA256.HashDataAsync(byteStream, ctn);
            return TokenHash.Create(Convert.ToHexString(bytes));
        }
    }
}
