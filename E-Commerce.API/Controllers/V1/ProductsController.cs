using Asp.Versioning;
using E_Commerce.API.Configuration;
using E_Commerce.API.Common.Pagination;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.ProductsRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Common.Dtos;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Product.Commands;
using E_Commerce.Application.Features.Product.Commands.CreateProduct;
using E_Commerce.Application.Features.Product.Commands.DeleteProduct;
using E_Commerce.Application.Features.Product.Commands.UpdateProduct;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Application.Features.Product.Queries;
using E_Commerce.Application.Features.Product.Queries.GetProducts;
using E_Commerce.Application.Features.Variant.Commands;
using E_Commerce.Application.Features.Variant.Commands.CreateVariant;
using E_Commerce.Application.Features.Variant.Commands.DeleteVariant;
using E_Commerce.Application.Features.Variant.Commands.UpdateVariant;
using E_Commerce.Application.Features.Variant.Common;
using E_Commerce.Application.Features.Variant.Queries;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/products")]
public sealed partial class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<ProductsResponse>> GetAll([FromQuery] PageApiRequest page, CancellationToken ct = default)
        => this.FromResult(await _sender.Send(new GetProductsQuery(new(page.PageNumber , page.PageSize)), ct), products => new ProductsResponse(products), "Products retrieved successfully.");

    [HttpGet("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<ProductResponse>> GetById(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetProductByIdQuery(id), ct), product => new ProductResponse(product), "Product retrieved successfully.");

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<ProductResponse>> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.CategoryId,
            request.Slug,
            request.Brand,
            request.BasePriceAmount,
            request.BasePriceCurrency,
            request.HasVariants,
            request.HasDiscount,
            request.CompareAtPriceAmount,
            request.CompareAtPriceCurrency,
            request.Variants?.Select(ToVariantCreateDto).ToList() ?? [],
            request.Status,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), product => new ProductResponse(product), "Product created successfully.", StatusCodes.Status201Created);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<ProductResponse>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.CategoryId,
            request.Slug,
            request.Brand,
            request.BasePriceAmount,
            request.BasePriceCurrency,
            request.HasVariants,
            request.HasDiscount,
            request.CompareAtPriceAmount,
            request.CompareAtPriceCurrency,
            request.Variants?.Select(ToVariantCreateDto).ToList(),
            request.Status,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), product => new ProductResponse(product), "Product updated successfully.");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult> Delete(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteProductCommand(id), ct), "Product deleted successfully.");

    [HttpGet("{productId:guid}/variants")]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<VariantsResponse>> GetVariants(Guid productId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetVariantsByProductIdQuery(productId), ct), variants => new VariantsResponse(variants), "Variants retrieved successfully.");

    [HttpGet("{productId:guid}/variants/{variantId:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<VariantResponse>> GetVariantById(Guid productId, Guid variantId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetVariantByIdQuery(productId, variantId), ct), variant => new VariantResponse(variant), "Variant retrieved successfully.");

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost("{productId:guid}/variants")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<VariantDetailsResponse>> CreateVariant(Guid productId, [FromBody] IReadOnlyCollection<CreateVariantRequest> request, CancellationToken ct)
    {
        var command = new CreateVariantCommand(
            productId,
            request?.Select(ToVariantCreateDto).ToList() ?? []);

        return this.FromResult(await _sender.Send(command, ct), variants => new VariantDetailsResponse(variants), "Variant created successfully.", StatusCodes.Status201Created);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{productId:guid}/variants/{variantId:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<VariantResponse>> UpdateVariant(Guid productId, Guid variantId, [FromBody] UpdateVariantRequest request, CancellationToken ct)
    {
        var command = new UpdateVariantCommand(
            productId,
            variantId,
            request.Sku,
            request.Size,
            request.Color is null ? null : new ColorDto(request.Color.Name, request.Color.HexCode),
            request.HasPriceOverride,
            request.VariantPriceOverrideAmount,
            request.IsDefault,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), variant => new VariantResponse(variant), "Variant updated successfully.");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{productId:guid}/variants/{variantId:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult> DeleteVariant(Guid productId, Guid variantId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteVariantCommand(productId, variantId), ct), "Variant deleted successfully.");

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPatch("{productId:guid}/variants/{variantId:guid}/stock-movement")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult> UpdateStockMovement(Guid productId , Guid variantId , [FromBody] UpdateStockMovementRequest request, CancellationToken ct)
    => this.FromResult(await _sender.Send(new UpdateStockMovementCommand(productId, variantId , request.Type , request.Quantity , request.Reason), ct), "Stock updated successfully.");

    private static VariantCreateDto ToVariantCreateDto(ProductVariantRequest request)
        => new(
            request.Sku,
            request.Size,
            request.Color is null ? null : new ColorDto(request.Color.Name, request.Color.HexCode),
            request.VariantPriceOverrideAmount,
            request.Stock,
            request.IsDefault,
            request.IsActive);

    private static VariantCreateDto ToVariantCreateDto(CreateVariantRequest request)
        => new(
            request.Sku,
            request.Size,
            request.Color is null ? null : new ColorDto(request.Color.Name, request.Color.HexCode),
            request.VariantPriceOverrideAmount,
            request.Stock,
            request.IsDefault,
            request.IsActive);
}
