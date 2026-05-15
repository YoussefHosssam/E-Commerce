namespace E_Commerce.API.Contracts.Requests.ImageRequests;

public sealed record ReorderImagesRequest(IReadOnlyCollection<ReorderImageItemRequest> Images);

public sealed record ReorderImageItemRequest(Guid ImageId, int SortOrder);
