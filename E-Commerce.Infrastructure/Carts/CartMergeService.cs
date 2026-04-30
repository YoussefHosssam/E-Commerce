using E_Commerce.Application.Common.Result;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Features.Cart.Commands;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Carts
{
    internal class CartMergeService : ICartMergeService
    {
        private readonly ICartSessionService _session;
        private readonly IUnitOfWork _uow;
        public CartMergeService(ICartSessionService session, IUnitOfWork uow)
        {
            _session = session;
            _uow = uow;
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

        public async Task MergeCarts(Guid userId , string anonymousToken , CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var guestCart = await _uow.Carts.GetCartWithItemsByToken(anonymousToken, ct);
            if (guestCart is null) return;

            var userCart = await _uow.Carts.GetCartWithItemsByUserId(userId, ct);

            if (userCart is null)
            {
                guestCart.AssignToUser(userId, now);
                await _uow.SaveChangesAsync(ct);
                return;
            }

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
            return;
        }
    }
}
