using E_Commerce.Domain.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Exceptions
{
    public class InfrastructureException : AppException
    {
        public InfrastructureException(string? code) : base(ErrorCatalog.FromCode(code))
        {
        }
    }
}
