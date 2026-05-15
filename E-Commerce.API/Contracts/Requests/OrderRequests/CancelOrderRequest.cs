namespace E_Commerce.API.Contracts.Requests.OrderRequests
{
    public record CancelOrderRequest(string? reason = null);
}
