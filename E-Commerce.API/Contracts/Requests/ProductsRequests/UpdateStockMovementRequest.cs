using E_Commerce.Domain.Enums;

namespace E_Commerce.API.Contracts.Requests.ProductsRequests
{
    public record UpdateStockMovementRequest(Guid VariantId,
        StockMovementType Type,
        int Quantity,
        string? Reason);
}
