namespace E_Commerce.API.Contracts.Requests.ImageRequests;

public sealed record GenerateImageUploadSignatureRequest(
    string ContentType,
    long SizeInBytes);
