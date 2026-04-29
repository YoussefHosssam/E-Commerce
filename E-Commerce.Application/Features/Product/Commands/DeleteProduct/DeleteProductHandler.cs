using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Product.Commands.DeleteProduct;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteProductHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.Id, true, cancellationToken);
        if (product is null)
        {
            return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.NotFound));
        }

        if (product.Variants.Count > 0)
        {
            return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.HasVariants));
        }

        await _uow.Products.DeleteAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

