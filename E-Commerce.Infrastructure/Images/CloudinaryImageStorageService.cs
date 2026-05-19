using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Domain.Entities;
using Microsoft.Extensions.Options;
using Polly;

namespace E_Commerce.Infrastructure.Images;

internal sealed class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ImageStorageOptions _options;
    private readonly ResiliencePipeline _pipeline;


    public CloudinaryImageStorageService(IOptions<ImageStorageOptions> options, ResiliencePipeline pipeline)
    {
        _options = options.Value;
        _cloudinary = new Cloudinary(new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret));
        _pipeline = pipeline;
    }

    public Task<GenerateImageUploadSignatureResponse> GenerateUploadSignatureAsync(
        GenerateImageUploadSignatureRequest request,
        CancellationToken cancellationToken)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var parameters = new SortedDictionary<string, object>
        {
            ["public_id"] = request.StorageKey,
            ["timestamp"] = timestamp
        };

        var signature = _cloudinary.Api.SignParameters(parameters);
        var uploadUrl = $"https://api.cloudinary.com/v1_1/{_options.CloudName}/image/upload";

        return Task.FromResult(new GenerateImageUploadSignatureResponse(
            _options.CloudName,
            _options.ApiKey,
            timestamp,
            signature,
            request.StorageKey,
            request.StorageKey,
            uploadUrl));
    }

    public async Task<ImageUploadVerificationResult?> GetImageResourceAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var result = await _pipeline.ExecuteAsync(async token =>
        {
            return await _cloudinary.GetResourceAsync(new GetResourceParams(storageKey)
            {
                ResourceType = ResourceType.Image
            });

        }, cancellationToken);

        if (result is null || result.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        return new ImageUploadVerificationResult(
            result.PublicId,
            result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty,
            result.Width,
            result.Height,
            result.Bytes,
            result.Format ?? string.Empty);
    }

    public async Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        await _pipeline.ExecuteAsync(
                async token =>
                {
                    await _cloudinary.DestroyAsync(new DeletionParams(storageKey)
                    {
                        ResourceType = ResourceType.Image,
                        Invalidate = true
                    });
                },
                cancellationToken);
    }

    public string BuildStorageKey<T>(Guid id)
    {
        if (typeof(T) == typeof(ProductImage))
            return ($"{_options.UploadFolderRoot.Trim('/')}/products/{id:N}/images/{Guid.NewGuid():N}");
        if (typeof(T) == typeof(VariantImage))
            return ($"{_options.UploadFolderRoot.Trim('/')}/variants/{id:N}/images/{Guid.NewGuid():N}");
        else
            return "";
    }
}
