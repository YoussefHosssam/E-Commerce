using AutoMapper;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace E_Commerce.Application.Features.ImageUploads.VariantImages;

internal sealed class GenerateVariantImageUploadSignatureHandler
    : IRequestHandler<GenerateVariantImageUploadSignatureCommand, Result<GenerateImageUploadSignatureResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;
    private readonly ImageUploadValidationService _validation;
    private readonly ImageStorageOptions _options;

    public GenerateVariantImageUploadSignatureHandler(
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
        GenerateVariantImageUploadSignatureCommand request,
        CancellationToken cancellationToken)
    {
        var requestError = _validation.ValidateRequestedFile(request.ContentType, request.SizeInBytes);
        if (requestError is not null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(requestError);

        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, true, cancellationToken);
        if (variant is null)
            return Result<GenerateImageUploadSignatureResponse>.Fail(VariantErrors.NotFound);

        var storageKey = BuildStorageKey(request.VariantId);
        var sortOrder = variant.Images.Count(x => x.ProcessingStatus != Domain.Enums.ImageProcessingStatus.Deleted) + 1;
        var isPrimary = variant.Images.All(x => x.ProcessingStatus == Domain.Enums.ImageProcessingStatus.Deleted);
        var image = VariantImage.CreatePending(request.VariantId, storageKey, isPrimary, sortOrder);

        variant.AddImage(image);
        await _uow.SaveChangesAsync(cancellationToken);

        var signature = await _imageStorage.GenerateUploadSignatureAsync(
            new GenerateImageUploadSignatureRequest(storageKey),
            cancellationToken);

        return Result<GenerateImageUploadSignatureResponse>.Success(signature);
    }

    private string BuildStorageKey(Guid variantId)
        => $"{_options.UploadFolderRoot.TrimEnd('/')}/variants/{variantId:N}/images/{Guid.NewGuid():N}";
}

internal sealed class CompleteVariantImageUploadHandler
    : IRequestHandler<CompleteVariantImageUploadCommand, Result<ImageDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;
    private readonly ImageUploadValidationService _validation;
    private readonly ImageStorageOptions _options;
    private readonly IMapper _mapper;

    public CompleteVariantImageUploadHandler(
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
        CompleteVariantImageUploadCommand request,
        CancellationToken cancellationToken)
    {
        var expectedPrefix = $"{_options.UploadFolderRoot.TrimEnd('/')}/variants/{request.VariantId:N}/images/";
        if (!_validation.HasExpectedPrefix(request.StorageKey, expectedPrefix))
            return Result<ImageDto>.Fail(ImageUploadErrors.StorageKeyInvalid);

        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, true, cancellationToken);
        if (variant is null)
            return Result<ImageDto>.Fail(VariantErrors.NotFound);

        var image = variant.GetImageByStorageKey(request.StorageKey);
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

internal sealed class DeleteVariantImageHandler : IRequestHandler<DeleteVariantImageCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IImageStorageService _imageStorage;

    public DeleteVariantImageHandler(IUnitOfWork uow, IImageStorageService imageStorage)
    {
        _uow = uow;
        _imageStorage = imageStorage;
    }

    public async Task<Result> Handle(DeleteVariantImageCommand request, CancellationToken cancellationToken)
    {
        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, true, cancellationToken);
        if (variant is null)
            return Result.Fail(VariantErrors.NotFound);

        var image = variant.GetImage(request.ImageId);
        await _imageStorage.DeleteAsync(image.StorageKey, cancellationToken);
        variant.DeleteImage(request.ImageId);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class SetPrimaryVariantImageHandler : IRequestHandler<SetPrimaryVariantImageCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public SetPrimaryVariantImageHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(SetPrimaryVariantImageCommand request, CancellationToken cancellationToken)
    {
        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, true, cancellationToken);
        if (variant is null)
            return Result.Fail(VariantErrors.NotFound);

        variant.SetPrimaryImage(request.ImageId);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

internal sealed class ReorderVariantImagesHandler : IRequestHandler<ReorderVariantImagesCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public ReorderVariantImagesHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(ReorderVariantImagesCommand request, CancellationToken cancellationToken)
    {
        var variant = await _uow.Variants.GetByIdWithDetailsAsync(request.VariantId, true, cancellationToken);
        if (variant is null)
            return Result.Fail(VariantErrors.NotFound);

        variant.ReorderImages(request.SortOrders);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
