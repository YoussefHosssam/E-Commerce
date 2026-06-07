namespace E_Commerce.API.Common.Pagination
{
    public sealed record PageApiRequest (int PageNumber = 1, int PageSize = 20)
    {
    }
}
