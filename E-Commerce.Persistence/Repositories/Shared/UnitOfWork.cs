using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Domain.Common.Errors;
using E_Commerce.Domain.Entities;
using E_Commerce.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Persistence.Repositories.Shared;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly EcommerceContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IVariantRepository Variants { get; }
    public IUserRepository Users { get; }
    public IOrderRepository Orders { get; }
    public IPaymentRepository Payments { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IGenericRepository<EmailMessage> EmailMessages { get; }
    public IIdempotencyRecordRepository IdempotencyRecords { get; }
    public IInventoryRepository Inventories { get; }
    public IStockMovementRepository StockMovements { get; }
    public IGenericRepository<UserTwoFactor> User2fa { get; }
    public ICartRepository Carts { get; }
    public IGenericRepository<CartItem> CartItems { get; }
    public IGenericRepository<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; }
    public IGenericRepository<TwoFactorLoginChallenge> TwoFactorLoginChallenges { get; }
    public IAuthTokenRepository AuthTokens { get; }
    public IGenericRepository<UserCredential> UserCredentials { get; }
    public IGenericRepository<UserOAuthAccount> UserOauthAccounts { get; }

    public IPaymentAttemptRepository PaymentAttempts { get; }

    public UnitOfWork(
        EcommerceContext context,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IVariantRepository variantRepository,
        IUserRepository users,
        IGenericRepository<EmailMessage> emailMessages,
        IGenericRepository<UserTwoFactor> user2fa,
        IGenericRepository<UserCredential> userCredentials,
        IGenericRepository<UserOAuthAccount> userOauthAccounts,
        IRefreshTokenRepository refreshTokens,
        IAuthTokenRepository authTokens,
        ICartRepository carts,
        IGenericRepository<CartItem> cartItems,
        IGenericRepository<TwoFactorRecoveryCode> twoFactorRecoveryCodes,
        IGenericRepository<TwoFactorLoginChallenge> twoFactorLoginChallenges,
        IInventoryRepository inventories,
        IStockMovementRepository stockMovements,
        IOrderRepository orders,
        IPaymentRepository payments,
        IPaymentAttemptRepository paymentAttempts,
        IIdempotencyRecordRepository idempotencyRecords,
        ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
        Products = productRepository;
        Categories = categoryRepository;
        Variants = variantRepository;
        Users = users;
        EmailMessages = emailMessages;
        User2fa = user2fa;
        UserCredentials = userCredentials;
        UserOauthAccounts = userOauthAccounts;
        RefreshTokens = refreshTokens;
        AuthTokens = authTokens;
        TwoFactorRecoveryCodes = twoFactorRecoveryCodes;
        TwoFactorLoginChallenges = twoFactorLoginChallenges;
        Inventories = inventories;
        Carts = carts;
        CartItems = cartItems;
        StockMovements = stockMovements;
        Orders = orders;
        Payments = payments;
        PaymentAttempts = paymentAttempts;
        IdempotencyRecords = idempotencyRecords;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException sqlException && (sqlException.Number == 4060 || sqlException.Number == 53 || sqlException.Number == -2))
        {
            _logger.LogError(
                exception,
                "Database unavailable during SaveChanges with SQL error {SqlErrorNumber}",
                sqlException.Number);

            throw new AppException(InfrastructureErrors.DatabaseUnavailable, exception);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Persistence failure during SaveChanges");

            throw new AppException(InfrastructureErrors.PersistenceFailure, exception);
        }
    }

    public ValueTask DisposeAsync()
        => _context.DisposeAsync();
}
