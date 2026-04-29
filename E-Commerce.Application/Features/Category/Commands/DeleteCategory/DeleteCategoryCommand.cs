using E_Commerce.Application.Common.Result;
using MediatR;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
