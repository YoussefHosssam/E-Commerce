using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using System.Diagnostics;

namespace E_Commerce.Infrastructure.Images;

internal sealed class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly ImageStorageOptions _options;
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<CloudinaryImageStorageService> _logger;


    public CloudinaryImageStorageService(
        IOptions<ImageStorageOptions> options,
        ResiliencePipelineProvider<string> pipelineProvider,
        ILogger<CloudinaryImageStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _cloudinary = new Cloudinary(new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret));
        _pipeline = pipelineProvider.GetPipeline("cloudinary");
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

        _logger.LogInformation(
            "Cloudinary upload signature generated for StorageKey {StorageKey}",
            request.StorageKey);

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
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Cloudinary resource lookup started for StorageKey {StorageKey}",
            storageKey);

        try
        {
            var result = await _pipeline.ExecuteAsync(async token =>
            {
                return await _cloudinary.GetResourceAsync(new GetResourceParams(storageKey)
                {
                    ResourceType = ResourceType.Image
                });

            }, cancellationToken);

            stopwatch.Stop();

            if (result is null || result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "Cloudinary resource not found for StorageKey {StorageKey} in {ElapsedMs} ms",
                    storageKey,
                    stopwatch.ElapsedMilliseconds);

                return null;
            }

            _logger.LogInformation(
                "Cloudinary resource lookup completed for StorageKey {StorageKey} in {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);

            return new ImageUploadVerificationResult(
                result.PublicId,
                result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty,
                result.Width,
                result.Height,
                result.Bytes,
                result.Format ?? string.Empty);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "Cloudinary resource lookup canceled for StorageKey {StorageKey} after {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Cloudinary resource lookup failed for StorageKey {StorageKey} after {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    public async Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Cloudinary resource delete started for StorageKey {StorageKey}",
            storageKey);

        try
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

            stopwatch.Stop();

            _logger.LogInformation(
                "Cloudinary resource delete completed for StorageKey {StorageKey} in {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();

            _logger.LogWarning(
                "Cloudinary resource delete canceled for StorageKey {StorageKey} after {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            _logger.LogError(
                exception,
                "Cloudinary resource delete failed for StorageKey {StorageKey} after {ElapsedMs} ms",
                storageKey,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
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
