using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Features.Cart.Commands;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class CartMergeService : ICartMergeService
    {
        private readonly ICartSessionService _session;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CartMergeService> _logger;

        public CartMergeService(
            ICartSessionService session,
            IUnitOfWork uow,
            ILogger<CartMergeService> logger)
        {
            _session = session;
            _uow = uow;
            _logger = logger;
        }

        public (bool, string) IsNeedToBeMerged()
        {
            var anonToken = _session.GetAnonymousId();
            if (string.IsNullOrEmpty(anonToken))
            {
                return (false, string.Empty);
            }
            return (true, anonToken);
        }

        public async Task MergeCarts(Guid userId, string anonymousToken, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Cart merge started for User {UserId}",
                userId);

            var guestCart = await _uow.Carts.GetCartWithItemsByToken(anonymousToken, ct);
            if (guestCart is null)
            {
                _logger.LogWarning(
                    "Cart merge skipped for User {UserId} because guest cart was not found",
                    userId);

                return;
            }

            var userCart = await _uow.Carts.GetCartWithItemsByUserId(userId, ct);

            if (userCart is null)
            {
                guestCart.AssignToUser(userId, now);
                await _uow.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "Cart merge completed for User {UserId}; guest Cart {CartId} assigned to user",
                    userId,
                    guestCart.Id);

                return;
            }

            var mergedItemCount = guestCart.Items.Count;

            foreach (var guestItem in guestCart.Items.ToList())
            {
                var existingUserItem = userCart.Items
                    .FirstOrDefault(x => x.VariantId == guestItem.VariantId);

                var finalQuantity = guestItem.Quantity + (existingUserItem?.Quantity ?? 0);
                var existQuantity = await _uow.Inventories.GetQuantityForVariant(guestItem.VariantId, ct);
                finalQuantity = Math.Min(finalQuantity, existQuantity);

                if (existingUserItem is not null)
                {
                    existingUserItem.SetQuantity(finalQuantity, now);
                }
                else
                {
                    var newItem = CartItem.Create(
                        userCart.Id,
                        guestItem.VariantId,
                        finalQuantity,
                        now);

                    userCart.AddItem(newItem, now);
                }
            }
            guestCart.SetStatus(CartStatus.Merged, now);
            await _uow.SaveChangesAsync(ct);

            _logger.LogInformation(
                "Cart merge completed for User {UserId}; guest Cart {GuestCartId} merged into Cart {CartId} with {ItemCount} items",
                userId,
                guestCart.Id,
                userCart.Id,
                mergedItemCount);

            return;
        }
    }
}
