using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.ImageUploads.Common;
using MediatR;

namespace E_Commerce.Application.Features.ImageUploads.ProductImages;

public sealed record GenerateProductImageUploadSignatureCommand(
    Guid ProductId,
    string ContentType,
    long SizeInBytes) : IRequest<Result<GenerateImageUploadSignatureResponse>>;

public sealed record CompleteProductImageUploadCommand(
    Guid ProductId,
    string StorageKey) : IRequest<Result<ImageDto>>;

public sealed record DeleteProductImageCommand(
    Guid ProductId,
    Guid ImageId) : IRequest<Result>;

public sealed record SetPrimaryProductImageCommand(
    Guid ProductId,
    Guid ImageId) : IRequest<Result>;

public sealed record ReorderProductImagesCommand(
    Guid ProductId,
    IReadOnlyDictionary<Guid, int> SortOrders) : IRequest<Result>;
