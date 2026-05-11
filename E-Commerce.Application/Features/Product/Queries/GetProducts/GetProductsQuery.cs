using E_Commerce.Application.Common.Pagination;
using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Category.Queries;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Product.Queries.GetProducts;

public sealed record GetProductsQuery(PageRequest page) : IRequest<Result<List<ProductListItemDto>>>;