using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Pagination
{
    public sealed record PageRequest(int PageNumber, int PageSize)
    {
        public int Page => PageNumber < 1 ? 1 : PageNumber;
        public int Size => PageSize < 1 ? 20 : PageSize > 200 ? 200 : PageSize;
        public int Skip => (Page - 1) * Size;
    }

}
