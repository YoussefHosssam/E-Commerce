using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.ProductsRequests;
using E_Commerce.API.Filters;
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
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ApiResult<List<ProductListItemDto>>> GetAll([FromQuery] PageRequest page, CancellationToken ct = default)
        => this.FromResult(await _sender.Send(new GetProductsQuery(page), ct), "Products retrieved successfully.");

    [HttpGet("{id:guid}")]
    public async Task<ApiResult<ProductDetailDto>> GetById(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetProductByIdQuery(id), ct), "Product retrieved successfully.");

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPost]
    public async Task<ApiResult<ProductDetailDto>> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var command = new CreateProductCommand(
            request.CategoryId,
            request.Slug,
            request.Brand,
            request.BasePriceAmount,
            request.BasePriceCurrency,
            request.Status,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), "Product created successfully.", StatusCodes.Status201Created);
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPut("{id:guid}")]
    public async Task<ApiResult<ProductDetailDto>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            request.CategoryId,
            request.Slug,
            request.Brand,
            request.BasePriceAmount,
            request.BasePriceCurrency,
            request.Status,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), "Product updated successfully.");
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteProductCommand(id), ct), "Product deleted successfully.");

    [HttpGet("{productId:guid}/variants")]
    public async Task<ApiResult<IReadOnlyCollection<VariantListItemDto>>> GetVariants(Guid productId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetVariantsByProductIdQuery(productId), ct), "Variants retrieved successfully.");

    [HttpGet("{productId:guid}/variants/{variantId:guid}")]
    public async Task<ApiResult<VariantDetailDto>> GetVariantById(Guid productId, Guid variantId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetVariantByIdQuery(productId, variantId), ct), "Variant retrieved successfully.");

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPost("{productId:guid}/variants")]
    public async Task<ApiResult<VariantDetailDto>> CreateVariant(Guid productId, [FromBody] CreateVariantRequest request, CancellationToken ct)
    {
        var command = new CreateVariantCommand(
            productId,
            request.Sku,
            request.Size,
            request.Color,
            request.PriceOverrideAmount,
            request.PriceOverrideCurrency,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), "Variant created successfully.", StatusCodes.Status201Created);
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPut("{productId:guid}/variants/{variantId:guid}")]
    public async Task<ApiResult<VariantDetailDto>> UpdateVariant(Guid productId, Guid variantId, [FromBody] UpdateVariantRequest request, CancellationToken ct)
    {
        var command = new UpdateVariantCommand(
            productId,
            variantId,
            request.Sku,
            request.Size,
            request.Color,
            request.PriceOverrideAmount,
            request.PriceOverrideCurrency,
            request.IsActive);

        return this.FromResult(await _sender.Send(command, ct), "Variant updated successfully.");
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpDelete("{productId:guid}/variants/{variantId:guid}")]
    public async Task<ApiResult> DeleteVariant(Guid productId, Guid variantId, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteVariantCommand(productId, variantId), ct), "Variant deleted successfully.");
}
