using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Features.Order.Common;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Common.Errors.ErrorCodes;

namespace E_Commerce.Application.Features.Order.Commands.CancelOrder
{
    public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CancelOrderHandler> _logger;

        public CancelOrderHandler(
            IUserAccessor userAccessor,
            IUnitOfWork uow,
            ILogger<CancelOrderHandler> logger)
        {
            _userAccessor = userAccessor;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var userId = _userAccessor.UserId;
            var userRole = _userAccessor.Role;

            if (!userId.HasValue) 
                return Result.Fail(AuthErrors.InvalidToken);
            if (!userRole.HasValue)
                return Result.Fail(AuthErrors.InvalidToken);

            var order = await _uow.Orders.GetTrackingOrderByIdWithDetailsAsync(request.id, cancellationToken);

            if (order is null) 
                return Result.Fail(OrderErrors.NotFound);
            if (!(order.Status == OrderStatus.Pending)) 
                return Result.Fail(OrderErrors.CancelNotAllowed);

            if (userRole != UserRole.Admin &&
                order.UserId != userId.Value)
            {
                _logger.LogWarning(
                    "Order cancel forbidden for Order {OrderId}, User {UserId}, Role {UserRole}",
                    request.id,
                    userId.Value,
                    userRole.Value);

                return Result.Fail(OrderErrors.NotFound);
            }

            return await CancelOrder(request, order, userId.Value, now, cancellationToken);
        }

        private async Task<Result> CancelOrder(CancelOrderCommand request,Domain.Entities.Order order, Guid userId , DateTimeOffset now, CancellationToken cancellationToken)
        {
            order.Cancel(request.reason, now);

            var variantsIds = order.Items
                .Select(i => i.VariantId)
                .Distinct()
                .ToList();

            var inventoriesResult = await LoadInventoriesAsync(variantsIds , order.Items, cancellationToken);

            if (!inventoriesResult.IsSuccess)
                return Result.Fail(inventoriesResult.Error!);

            var inventoriesByVariantIds = inventoriesResult.Data!;
            List<Domain.Entities.StockMovement> stockMovements = new();

            string reason = string.IsNullOrEmpty(request.reason) ? "Order has been canceled." : request.reason;
            foreach (var orderItem in order.Items)
            {
                var inventory = inventoriesByVariantIds[orderItem.VariantId];
                inventory.ReleaseReservation(orderItem.Quantity, now);
                Domain.Entities.StockMovement stockMovement = Domain.Entities.StockMovement.Create(orderItem.VariantId, StockMovementType.ReservationReleased, orderItem.Quantity, reason, order.Id, userId, now);
                stockMovements.Add(stockMovement);
            }

            await _uow.StockMovements.CreateRangeAsync(stockMovements, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Order {OrderId} canceled by User {UserId}; released {ItemCount} inventory reservations",
                order.Id,
                userId,
                order.Items.Count);

            return Result.Success();
        }
        private async Task<Result<Dictionary<Guid, Domain.Entities.Inventory>>> LoadInventoriesAsync(List<Guid> variantIds , IReadOnlyCollection<Domain.Entities.OrderItem> orderItems,CancellationToken cancellationToken)
        {

            var inventories = await _uow.Inventories.GetByVariantIdsAsync(
                variantIds,
                cancellationToken);

            var inventoryByVariantId = inventories.ToDictionary(i => i.VariantId);

            foreach (var item in orderItems)
            {
                if (!inventoryByVariantId.TryGetValue(item.VariantId, out var inventory))
                    return Result<Dictionary<Guid, Domain.Entities.Inventory>>.Fail(CheckoutErrors.UnfoundInventory);
            }

            return Result<Dictionary<Guid, Domain.Entities.Inventory>>.Success(inventoryByVariantId);
        }
    }
}
