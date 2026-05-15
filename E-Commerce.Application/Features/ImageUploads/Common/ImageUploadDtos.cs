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

public sealed record ImageDto(
    Guid Id,
    string StorageKey,
    string Url,
    int Width,
    int Height,
    long SizeInBytes,
    string Format,
    bool IsPrimary,
    int SortOrder,
    string ProcessingStatus);
