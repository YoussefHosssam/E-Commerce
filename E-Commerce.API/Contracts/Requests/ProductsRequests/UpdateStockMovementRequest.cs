using E_Commerce.Domain.Enums;

namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public record UpdateStockMovementRequest(
        StockMovementType Type,
        int Quantity,
        string? Reason);
}
