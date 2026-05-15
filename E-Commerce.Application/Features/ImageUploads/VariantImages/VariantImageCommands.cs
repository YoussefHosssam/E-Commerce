using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.ImageUploads.Common;
using MediatR;

namespace E_Commerce.Application.Features.ImageUploads.VariantImages;

public sealed record GenerateVariantImageUploadSignatureCommand(
    Guid VariantId,
    string ContentType,
    long SizeInBytes) : IRequest<Result<GenerateImageUploadSignatureResponse>>;

public sealed record CompleteVariantImageUploadCommand(
    Guid VariantId,
    string StorageKey) : IRequest<Result<ImageDto>>;

public sealed record DeleteVariantImageCommand(
    Guid VariantId,
    Guid ImageId) : IRequest<Result>;

public sealed record SetPrimaryVariantImageCommand(
    Guid VariantId,
    Guid ImageId) : IRequest<Result>;

public sealed record ReorderVariantImagesCommand(
    Guid VariantId,
    IReadOnlyDictionary<Guid, int> SortOrders) : IRequest<Result>;
