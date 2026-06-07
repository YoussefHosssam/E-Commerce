namespace E_Commerce.Application.Features.ImageUploads.Common;

public sealed record GenerateImageUploadSignatureRequest(
    string StorageKey);

public sealed record GenerateImageUploadSignatureResponse(
    string CloudName,
    string ApiKey,
    long Timestamp,
    string Signature,
    string StorageKey,
    string PublicId,
    string UploadUrl);

public sealed record CompleteProductImageUploadRequest(
    Guid ProductId,
    string StorageKey);

public sealed record CompleteVariantImageUploadRequest(
    Guid VariantId,
    string StorageKey);

public sealed record ImageUploadVerificationResult(
    string StorageKey,
    string Url,
    int Width,
    int Height,
    long SizeInBytes,
    string Format);

public sealed record ImageDto
{
    public Guid Id { get; init; }
    public string StorageKey { get; init; } = default!;
    public string Url { get; init; } = default!;
    public int Width { get; init; }
    public int Height { get; init; }
    public long SizeInBytes { get; init; }
    public string Format { get; init; } = default!;
    public bool IsPrimary { get; init; }
    public int SortOrder { get; init; }
    public string ProcessingStatus { get; init; } = default!;
}
