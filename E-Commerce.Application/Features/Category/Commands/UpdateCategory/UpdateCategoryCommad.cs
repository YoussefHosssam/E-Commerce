using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Category.Common;
using MediatR;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed record UpdateCategoryCommand(
    Guid Id,
    Guid? ParentId,
    string Slug,
    int SortOrder,
    bool IsActive) : IRequest<Result<CategoryDetailDto>>;
