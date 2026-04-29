using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common.Pagination
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNext => PageNumber < TotalPages;
        public bool HasPrevious => PageNumber > 1;
        public object? ToMetaResult()
        {
            return new { PageNumber, PageSize, TotalCount, TotalPages, HasNext, HasPrevious };
        }
    }
}

