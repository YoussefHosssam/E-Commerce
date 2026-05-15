using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CategoriesRequests;
using E_Commerce.API.Filters;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Category.Commands;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Application.Features.Category.Queries;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ApiResult<IReadOnlyCollection<CategoryListItemDto>>> GetAll( [FromQuery] PageRequest page , CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetCategoriesQuery(page), ct), "Categories retrieved successfully.");

    [HttpGet("{id:guid}")]
    public async Task<ApiResult<CategoryDetailDto>> GetById(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new GetCategoryByIdQuery(id), ct), "Category retrieved successfully.");

    [HttpGet("{id:guid}/products")]
    public async Task<ApiResult<IReadOnlyCollection<ProductListItemDto>>> GetProductsById(Guid id , PageRequest page, CancellationToken ct)
    => this.FromResult(await _sender.Send(new GetCategoryProductsQuery(id , page), ct), "Products retrieved successfully.");

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPost]
    public async Task<ApiResult<CategoryDetailDto>> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var command = new CreateCategoryCommand(request.Name,request.ParentId,request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), "Category created successfully.", StatusCodes.Status201Created);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPut("{id:guid}")]
    public async Task<ApiResult<CategoryDetailDto>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(request.Name,id, request.ParentId, request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), "Category updated successfully.");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteCategoryCommand(id), ct), "Category deleted successfully.");
}
