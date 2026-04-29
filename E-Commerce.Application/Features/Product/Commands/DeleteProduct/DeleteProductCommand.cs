using E_Commerce.Application.Common.Result;
using MediatR;

namespace E_Commerce.Application.Features.Product.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Result>;

