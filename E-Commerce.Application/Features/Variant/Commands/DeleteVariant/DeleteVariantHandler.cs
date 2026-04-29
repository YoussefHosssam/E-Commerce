using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.DeleteVariant;

public sealed class DeleteVariantHandler : IRequestHandler<DeleteVariantCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteVariantHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteVariantCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdWithDetailsAsync(request.ProductId, true, cancellationToken);
        if (product is null)
        {
            return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Product.NotFound));
        }

        if (!product.Variants.Any(x => x.Id == request.VariantId))
        {
            return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Variant.NotFound));
        }

        if (await _uow.Variants.IsVariantReferencedAsync(request.VariantId, cancellationToken))
        {
            return Result.Fail(ErrorCatalog.FromCode(ErrorCodes.Variant.DeleteReferenced));
        }

        product.RemoveVariant(request.VariantId, DateTimeOffset.UtcNow);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

