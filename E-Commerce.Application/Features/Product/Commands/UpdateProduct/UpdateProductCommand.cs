using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Enums;
using MediatR;

namespace E_Commerce.Application.Features.Product.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    string Slug,
    string? Brand,
    decimal BasePriceAmount,
    string BasePriceCurrency,
    ProductStatus Status,
    bool IsActive) : IRequest<Result<ProductDetailDto>>;

