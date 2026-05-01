namespace E_Commerce.API.Contracts.Requests.CartRequests
{
    public sealed record AddItemRequest (Guid VariantId , int Quantity);
}
