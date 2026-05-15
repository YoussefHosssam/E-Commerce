using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Category.Common;
using MediatR;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentId,
    string Slug,
    int SortOrder,
    bool IsActive) : IRequest<Result<CategoryDetailDto>>;
