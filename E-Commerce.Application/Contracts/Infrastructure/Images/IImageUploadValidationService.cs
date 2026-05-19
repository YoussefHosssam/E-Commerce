using E_Commerce.Domain.Common.Errors;

namespace E_Commerce.Application.Features.ImageUploads.Common;

public interface IImageUploadValidationService
{
    Error? ValidateRequestedFile(string contentType, long sizeInBytes);

    Error? ValidateVerifiedResource(ImageUploadVerificationResult resource);

    bool HasExpectedPrefix<T>(string storageKey , Guid Id);
}