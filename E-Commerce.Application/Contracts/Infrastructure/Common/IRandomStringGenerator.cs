using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Common
{
    public interface IRandomStringGenerator
    {
        public string GetRandomString(int length = 8);
    }
}
