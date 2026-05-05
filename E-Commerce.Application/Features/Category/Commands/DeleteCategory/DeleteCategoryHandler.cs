using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Common.Errors;
using MediatR;

namespace E_Commerce.Application.Features.Category.Commands;

public sealed class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteCategoryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            return Result.Fail(CategoryErrors.NotFound);
        }

        if (await _uow.Categories.HasChildrenAsync(request.Id, cancellationToken))
        {
            return Result.Fail(CategoryErrors.DeleteHasChildren);
        }

        if (await _uow.Categories.HasProductsAsync(request.Id, cancellationToken))
        {
            return Result.Fail(CategoryErrors.DeleteHasProducts);
        }

        await _uow.Categories.DeleteAsync(request.Id, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

