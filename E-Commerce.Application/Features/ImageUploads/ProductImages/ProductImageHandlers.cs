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
    private readonly IImageUploadValidationService _validation;

    public GenerateProductImageUploadSignatureHandler(
        IUnitOfWork uow,
        IImageStorageService imageStorage,
        IImageUploadValidationService validation
        )
    {
        _uow = uow;
        _imageStorage = imageStorage;
        _validation = validation;
    }

    public async Task<Result<GenerateImageUploadSignatureResponse>> Handle(
        GenerateProductImageUploadSignatureCommand request,
        CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var requestError = _validation.ValidateRequestedFile(request.ContentType, request.SizeInBytes);
        if (requestError is not null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(requestError);

        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(ProductErrors.NotFound);
        if (product.Images.Count(i => i.ProcessingStatus == Domain.Enums.ImageProcessingStatus.Uploaded) > 10)
            return Result<GenerateImageUploadSignatureResponse>.Fail(ProductImageErrors.ExceedImagesLimit);

        var storageKey = _imageStorage.BuildStorageKey<ProductImage>(request.ProductId);
        var sortOrder = product.Images.Count(x => x.ProcessingStatus == Domain.Enums.ImageProcessingStatus.Uploaded) + 1;
        var isPrimary = product.Images.All(x => x.ProcessingStatus == Domain.Enums.ImageProcessingStatus.Deleted);
        var expiresAt = now.AddMinutes(15);
        var image = ProductImage.CreatePending(request.ProductId, storageKey, isPrimary, sortOrder , expiresAt);

        product.AddImage(image, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);

        var signature = await _imageStorage.GenerateUploadSignatureAsync(
            new GenerateImageUploadSignatureRequest(storageKey),
            cancellationToken);

        return Result<GenerateImageUploadSignatureResponse>.Success(signature);
    }
}

internal sealed class CompleteProductImageUploadHandler
    : IRequestHandler<CompleteProductImageUploadCommand, Result<ImageDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;
    private readonly IImageUploadValidationService _validation;
    private readonly IMapper _mapper;

    public CompleteProductImageUploadHandler(
        IUnitOfWork uow,
        IImageStorageService imageStorage,
        IImageUploadValidationService validation,
        IMapper mapper)
    {
        _uow = uow;
        _imageStorage = imageStorage;
        _validation = validation;
        _mapper = mapper;
    }

    public async Task<Result<ImageDto>> Handle(
        CompleteProductImageUploadCommand request,
        CancellationToken cancellationToken)
    {
        if (!_validation.HasExpectedPrefix<ProductImage>(request.StorageKey , request.ProductId))
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

        image.MarkUploaded(resource.Url, resource.Width, resource.Height, resource.SizeInBytes, resource.Format);
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
