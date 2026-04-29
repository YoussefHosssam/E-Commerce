using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.CategoriesRequests;
using E_Commerce.API.Filters;
using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Features.Category.Commands;
using E_Commerce.Application.Features.Category.Common;
using E_Commerce.Application.Features.Category.Queries;
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

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPost]
    public async Task<ApiResult<CategoryDetailDto>> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var command = new CreateCategoryCommand(request.ParentId, request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), "Category created successfully.", StatusCodes.Status201Created);
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpPut("{id:guid}")]
    public async Task<ApiResult<CategoryDetailDto>> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(id, request.ParentId, request.Slug, request.SortOrder, request.IsActive);
        return this.FromResult(await _sender.Send(command, ct), "Category updated successfully.");
    }

    [Authorize]
    [ServiceFilter(typeof(AdminRouteFilter))]
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult> Delete(Guid id, CancellationToken ct)
        => this.FromResult(await _sender.Send(new DeleteCategoryCommand(id), ct), "Category deleted successfully.");
}
