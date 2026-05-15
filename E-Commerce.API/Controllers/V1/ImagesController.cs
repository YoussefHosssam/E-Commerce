using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.ImageRequests;
using E_Commerce.API.Filters;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Application.Features.ImageUploads.ProductImages;
using E_Commerce.Application.Features.ImageUploads.VariantImages;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiGenerateImageUploadSignatureRequest = E_Commerce.API.Contracts.Requests.ImageRequests.GenerateImageUploadSignatureRequest;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Authorize]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/v{version:apiVersion}")]
public sealed class ImagesController : ControllerBase
{
    private readonly ISender _sender;

    public ImagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("products/{productId:guid}/images/upload-signature")]
    public async Task<ApiResult<GenerateImageUploadSignatureResponse>> GenerateProductImageUploadSignature(
        Guid productId,
        [FromBody] ApiGenerateImageUploadSignatureRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GenerateProductImageUploadSignatureCommand(productId, request.ContentType, request.SizeInBytes),
            ct);

        return this.FromResult(result, "Product image upload signature generated successfully.");
    }

    [HttpPost("products/{productId:guid}/images/complete")]
    public async Task<ApiResult<ImageDto>> CompleteProductImageUpload(
        Guid productId,
        [FromBody] CompleteImageUploadRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new CompleteProductImageUploadCommand(productId, request.StorageKey),
            ct);

        return this.FromResult(result, "Product image upload completed successfully.");
    }

    [HttpDelete("products/{productId:guid}/images/{imageId:guid}")]
    public async Task<ApiResult> DeleteProductImage(
        Guid productId,
        Guid imageId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteProductImageCommand(productId, imageId), ct);
        return this.FromResult(result, "Product image deleted successfully.");
    }

    [HttpPatch("products/{productId:guid}/images/{imageId:guid}/primary")]
    public async Task<ApiResult> SetPrimaryProductImage(
        Guid productId,
        Guid imageId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new SetPrimaryProductImageCommand(productId, imageId), ct);
        return this.FromResult(result, "Product primary image updated successfully.");
    }

    [HttpPatch("products/{productId:guid}/images/reorder")]
    public async Task<ApiResult> ReorderProductImages(
        Guid productId,
        [FromBody] ReorderImagesRequest request,
        CancellationToken ct)
    {
        var sortOrders = request.Images.ToDictionary(x => x.ImageId, x => x.SortOrder);
        var result = await _sender.Send(new ReorderProductImagesCommand(productId, sortOrders), ct);
        return this.FromResult(result, "Product images reordered successfully.");
    }

    [HttpPost("variants/{variantId:guid}/images/upload-signature")]
    public async Task<ApiResult<GenerateImageUploadSignatureResponse>> GenerateVariantImageUploadSignature(
        Guid variantId,
        [FromBody] ApiGenerateImageUploadSignatureRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new GenerateVariantImageUploadSignatureCommand(variantId, request.ContentType, request.SizeInBytes),
            ct);

        return this.FromResult(result, "Variant image upload signature generated successfully.");
    }

    [HttpPost("variants/{variantId:guid}/images/complete")]
    public async Task<ApiResult<ImageDto>> CompleteVariantImageUpload(
        Guid variantId,
        [FromBody] CompleteImageUploadRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new CompleteVariantImageUploadCommand(variantId, request.StorageKey),
            ct);

        return this.FromResult(result, "Variant image upload completed successfully.");
    }

    [HttpDelete("variants/{variantId:guid}/images/{imageId:guid}")]
    public async Task<ApiResult> DeleteVariantImage(
        Guid variantId,
        Guid imageId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteVariantImageCommand(variantId, imageId), ct);
        return this.FromResult(result, "Variant image deleted successfully.");
    }

    [HttpPatch("variants/{variantId:guid}/images/{imageId:guid}/primary")]
    public async Task<ApiResult> SetPrimaryVariantImage(
        Guid variantId,
        Guid imageId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new SetPrimaryVariantImageCommand(variantId, imageId), ct);
        return this.FromResult(result, "Variant primary image updated successfully.");
    }

    [HttpPatch("variants/{variantId:guid}/images/reorder")]
    public async Task<ApiResult> ReorderVariantImages(
        Guid variantId,
        [FromBody] ReorderImagesRequest request,
        CancellationToken ct)
    {
        var sortOrders = request.Images.ToDictionary(x => x.ImageId, x => x.SortOrder);
        var result = await _sender.Send(new ReorderVariantImagesCommand(variantId, sortOrders), ct);
        return this.FromResult(result, "Variant images reordered successfully.");
    }
}
