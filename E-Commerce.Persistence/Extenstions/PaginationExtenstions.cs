using E_Commerce.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest page,
        CancellationToken ct)
    {
        var total = await query.CountAsync(ct);

        var items = await query
            .Skip(page.Skip)
            .Take(page.Size)
            .ToListAsync(ct);

        return new PagedResult<T>(
            items,
            page.PageNumber,
            page.PageSize,
            total);
    }
}