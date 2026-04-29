using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Extensions;
using E_Commerce.Application.Features.Product.Common;
using E_Commerce.Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace E_Commerce.Application.Features.Product.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDetailDto>>;

public sealed class GetProductByIdValidation : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithError(ErrorCodes.Product.IdRequired);
    }
}

public sealed class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    private readonly IUnitOfWork _uow;

    public GetProductByIdHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.Id, false, cancellationToken);
        if (product is null)
        {
            return Result<ProductDetailDto>.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.NotFound));
        }

        return Result<ProductDetailDto>.Success(product.ToDetailDto());
    }
}
