using E_Commerce.Application.Features.ImageUploads.Common;

namespace E_Commerce.Application.Contracts.Infrastructure.Images;

public interface IImageStorageService
{
    Task<GenerateImageUploadSignatureResponse> GenerateUploadSignatureAsync(
        GenerateImageUploadSignatureRequest request,
        CancellationToken cancellationToken);

    Task<ImageUploadVerificationResult?> GetImageResourceAsync(
        string storageKey,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken);
}
