using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Enums
{
    public enum IdempotencyRequestStatus
    {
        Processing = 1,
        Completed = 2,
        Failed = 3
    }
}
