using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts.Infrastructure.Idempotency
{
    public enum IdempotencyStatus
    {
        Processing = 1,
        Completed = 2,
        Failed = 3
    }
}
