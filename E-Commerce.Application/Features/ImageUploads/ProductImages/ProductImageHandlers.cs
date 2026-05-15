using AutoMapper;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Features.ImageUploads.ProductImages;

internal sealed class GenerateProductImageUploadSignatureHandler
    : IRequestHandler<GenerateProductImageUploadSignatureCommand, Result<GenerateImageUploadSignatureResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;
    private readonly ImageUploadValidationService _validation;
    private readonly ImageStorageOptions _options;

    public GenerateProductImageUploadSignatureHandler(
        IUnitOfWork uow,
        IImageStorageService imageStorage,
        ImageUploadValidationService validation,
        IOptions<ImageStorageOptions> options)
    {
        _uow = uow;
        _imageStorage = imageStorage;
        _validation = validation;
        _options = options.Value;
    }

    public async Task<Result<GenerateImageUploadSignatureResponse>> Handle(
        GenerateProductImageUploadSignatureCommand request,
        CancellationToken cancellationToken)
    {
        var requestError = _validation.ValidateRequestedFile(request.ContentType, request.SizeInBytes);
        if (requestError is not null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(requestError);

        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(ProductErrors.NotFound);

        var storageKey = BuildStorageKey(request.ProductId);
        var sortOrder = product.Images.Count(x => x.ProcessingStatus != Domain.Enums.ImageProcessingStatus.Deleted) + 1;
        var isPrimary = product.Images.All(x => x.ProcessingStatus == Domain.Enums.ImageProcessingStatus.Deleted);
        var image = ProductImage.CreatePending(request.ProductId, storageKey, isPrimary, sortOrder);

        product.AddImage(image, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        var signature = await _imageStorage.GenerateUploadSignatureAsync(
            new GenerateImageUploadSignatureRequest(storageKey),
            cancellationToken);

        return Result<GenerateImageUploadSignatureResponse>.Success(signature);
    }

    private string BuildStorageKey(Guid productId)
        => $"{_options.UploadFolderRoot.TrimEnd('/')}/products/{productId:N}/images/{Guid.NewGuid():N}";
}

internal sealed class CompleteProductImageUploadHandler
    : IRequestHandler<CompleteProductImageUploadCommand, Result<ImageDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;
    private readonly ImageUploadValidationService _validation;
    private readonly ImageStorageOptions _options;
    private readonly IMapper _mapper;

    public CompleteProductImageUploadHandler(
        IUnitOfWork uow,
        IImageStorageService imageStorage,
        ImageUploadValidationService validation,
        IOptions<ImageStorageOptions> options,
        IMapper mapper)
    {
        _uow = uow;
        _imageStorage = imageStorage;
        _validation = validation;
        _options = options.Value;
        _mapper = mapper;
    }

    public async Task<Result<ImageDto>> Handle(
        CompleteProductImageUploadCommand request,
        CancellationToken cancellationToken)
    {
        var expectedPrefix = $"{_options.UploadFolderRoot.TrimEnd('/')}/products/{request.ProductId:N}/images/";
        if (!_validation.HasExpectedPrefix(request.StorageKey, expectedPrefix))
            return Result<ImageDto>.Fail(ImageUploadErrors.StorageKeyInvalid);

        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result<ImageDto>.Fail(ProductErrors.NotFound);

        var image = product.GetImageByStorageKey(request.StorageKey);
        var resource = await _imageStorage.GetImageResourceAsync(request.StorageKey, cancellationToken);

        if (resource is null)
        {
            image.MarkFailed();
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<ImageDto>.Fail(ImageUploadErrors.UploadedResourceNotFound);
        }

        var verificationError = _validation.ValidateVerifiedResource(resource);
        if (verificationError is not null)
        {
            image.MarkFailed();
            await _uow.SaveChangesAsync(cancellationToken);
            await _imageStorage.DeleteAsync(request.StorageKey, cancellationToken);
            return Result<ImageDto>.Fail(verificationError);
        }

        image.MarkReady(resource.Url, resource.Width, resource.Height, resource.SizeInBytes, resource.Format);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<ImageDto>.Success(_mapper.Map<ImageDto>(image));
    }
}

internal sealed class DeleteProductImageHandler : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;

    public DeleteProductImageHandler(IUnitOfWork uow, IImageStorageService imageStorage)
    {
        _uow = uow;
        _imageStorage = imageStorage;
    }

    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result.Fail(ProductErrors.NotFound);

        var image = product.GetImage(request.ImageId);
        await _imageStorage.DeleteAsync(image.StorageKey, cancellationToken);
        product.DeleteImage(request.ImageId, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class SetPrimaryProductImageHandler : IRequestHandler<SetPrimaryProductImageCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public SetPrimaryProductImageHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(SetPrimaryProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result.Fail(ProductErrors.NotFound);

        product.SetPrimaryImage(request.ImageId, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class ReorderProductImagesHandler : IRequestHandler<ReorderProductImagesCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public ReorderProductImagesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(ReorderProductImagesCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result.Fail(ProductErrors.NotFound);

        product.ReorderImages(request.SortOrders, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
