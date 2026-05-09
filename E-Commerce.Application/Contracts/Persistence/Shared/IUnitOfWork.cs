using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Entities;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IVariantRepository Variants { get; }
    IUserRepository Users { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IIdempotencyRecordRepository IdempotencyRecords { get; }
    IPaymentRepository Payments { get; }
    IPaymentAttemptRepository PaymentAttempts { get; }
    IGenericRepository<CartItem> CartItems { get; }
    IGenericRepository<UserTwoFactor> User2fa { get; }
    IGenericRepository<UserCredential> UserCredentials { get; }
    IInventoryRepository Inventories { get; }
    IStockMovementRepository StockMovements { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IGenericRepository<EmailMessage> EmailMessages { get; }
    IGenericRepository<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; }
    IGenericRepository<TwoFactorLoginChallenge> TwoFactorLoginChallenges { get; }
    IAuthTokenRepository AuthTokens { get; }
    IGenericRepository<UserOAuthAccount> UserOauthAccounts { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}
