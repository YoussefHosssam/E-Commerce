using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using MediatR;

namespace E_Commerce.Application.Features.Variant.Commands.UpdateVariant;

internal class UpdateStockMovementHandlder
    : IRequestHandler<UpdateStockMovementCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly IUserAccessor _userAccessor;

    public UpdateStockMovementHandlder(
        IUnitOfWork uow,
        IUserAccessor userAccessor)
    {
        _uow = uow;
        _userAccessor = userAccessor;
    }

    public async Task<Result> Handle(
        UpdateStockMovementCommand request,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        Inventory? inventory = await _uow.Inventories
            .GetByVariantIdAsync(request.VariantId, cancellationToken);

        if (inventory is null)
        {
            return Result.Fail(InventoryErrors.VariantIdRequired);
        }

        int delta = 0;

        switch (request.Type)
        {
            case StockMovementType.StockIn:
                inventory.AddStock(request.Quantity, now);
                delta = request.Quantity;
                break;

            case StockMovementType.StockOut:
            case StockMovementType.Damage:
                inventory.AdjustStock(inventory.OnHand - request.Quantity, now);
                delta = -request.Quantity;
                break;

            case StockMovementType.Return:
                inventory.AddStock(request.Quantity, now);
                delta = request.Quantity;
                break;

            case StockMovementType.Adjustment:
                inventory.AdjustStock(inventory.OnHand + request.Quantity, now);
                delta = request.Quantity;
                break;

            default:
                return Result.Fail(StockMovementErrors.TypeInvalid);
        }

        var movement = StockMovement.Create(
            request.VariantId,
            request.Type,
            delta,
            request.Reason,
            refId: null,
            actorUserId: _userAccessor.UserId,
            now);

        await _uow.StockMovements.CreateAsync(movement, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}