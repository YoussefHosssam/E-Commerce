using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Pagination
{
    public sealed record PageRequest(int PageNumber = 1, int PageSize = 20)
    {
        private int Page => PageNumber < 1 ? 1 : PageNumber;
        private int Size => PageSize < 1 ? 20 : PageSize > 200 ? 200 : PageSize; // clamp
        private int Skip => (Page - 1) * Size;
    }

}
