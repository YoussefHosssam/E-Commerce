using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Common
{
    public interface ITokenGenerator
    {
        public Task<string> GenerateTokenAsync(int size = 32);
    }
}
