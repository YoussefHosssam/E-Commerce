using Asp.Versioning;
using E_Commerce.API.Configuration;
using E_Commerce.API.Common.Pagination;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CategoriesRequests;
using E_Commerce.API.Contracts.Responses;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Category.Commands;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Application.Features.Category.Queries;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/categories")]
public sealed partial class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<CategoriesResponse>> GetAll( [FromQuery] PageApiRequest page , CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetCategoriesQuery(new(page.PageNumber, page.PageSize)), ct), categories => new CategoriesResponse(categories), "Categories retrieved successfully.");

    [HttpGet("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<CategoryResponse>> GetById(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetCategoryByIdQuery(id), ct), category => new CategoryResponse(category), "Category retrieved successfully.");

    [HttpGet("{id:guid}/products")]
    [EnableRateLimiting(RateLimitingConfiguration.PublicLimiter)]
    public async Task<ApiResult<ProductsResponse>> GetProductsById(Guid id , [FromQuery] PageApiRequest page, CancellationToken ct)
    => this.FromResult(await _sender.Send(new GetCategoryProductsQuery(id , new(page.PageNumber, page.PageSize)), ct), products => new ProductsResponse(products), "Products retrieved successfully.");

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var command = new CreateCategoryCommand(request.Name,request.ParentId,request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), category => new CategoryResponse(category), "Category created successfully.", StatusCodes.Status201Created);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult<CategoryResponse>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(request.Name,id, request.ParentId, request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), category => new CategoryResponse(category), "Category updated successfully.");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}")]
    [EnableRateLimiting(RateLimitingConfiguration.ExpensiveLimiter)]
    public async Task<ApiResult> Delete(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteCategoryCommand(id), ct), "Category deleted successfully.");
}
