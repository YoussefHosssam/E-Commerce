using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Application.Features.Cart.Common;
using E_Commerce.Application.Features.Checkout.Common;
using E_Commerce.Domain.Entities;

namespace E_Commerce.Application.Contracts.Persistence;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartWithItemsByToken(string token, CancellationToken ctn);
    Task<Cart?> GetCartWithItemsByUserId(Guid id, CancellationToken ctn);
    Task<CartSummaryDTO?> GetCartSummaryDtoByTokenAsync(string token, CancellationToken ctn);
    Task<CartSummaryDTO?> GetCartSummaryDtoByUserIdAsync(Guid id, CancellationToken ctn);
    Task<CheckoutSummaryDto?> GetCheckoutSummaryDtoByUserIdAsync(Guid id, CancellationToken ctn);
}
