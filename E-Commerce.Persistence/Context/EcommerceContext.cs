using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace E_Commerce.Persistence.Context
{
    public class EcommerceContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserTwoFactor> Users2Fa { get; set; }
        public DbSet<UserOAuthAccount> UserOAuthAccounts { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<VariantImage> VariantImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }

        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<UserCredential> UserCredentials { get; set; }
        public DbSet<TwoFactorLoginChallenge> TwoFactorLoginChallenges { get; set; }
        public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; set; }
        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }

        public EcommerceContext(DbContextOptions<EcommerceContext> opt) : base (opt)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

    }
}
