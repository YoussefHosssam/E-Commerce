using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Features.ImageUploads.Common;
using Microsoft.Extensions.Options;

namespace E_Commerce.Infrastructure.Images;

internal sealed class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ImageStorageOptions _options;

    public CloudinaryImageStorageService(IOptions<ImageStorageOptions> options)
    {
        _options = options.Value;
        _cloudinary = new Cloudinary(new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret));
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
        var result = await _cloudinary.GetResourceAsync(
            new GetResourceParams(storageKey)
            {
                ResourceType = ResourceType.Image
            });

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
        await _cloudinary.DestroyAsync(new DeletionParams(storageKey)
        {
            ResourceType = ResourceType.Image,
            Invalidate = true
        });
    }
}
