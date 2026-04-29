using E_Commerce.Application.Contracts.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Common
{
    internal class TokenGenerator : ITokenGenerator
    {
        public Task<string> GenerateTokenAsync(int size = 32)
        {
            byte[] bytes = new byte[size];
            RandomNumberGenerator.Fill(bytes);

            string token = Convert.ToBase64String(bytes);

            return Task.FromResult(token);
        }
    }
}
