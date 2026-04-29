using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Product.Common;
using MediatR;

namespace E_Commerce.Application.Features.Product.Queries.GetProducts;

public sealed record GetProductsQuery(PageRequest page) : IRequest<Result<List<ProductListItemDto>>>;
