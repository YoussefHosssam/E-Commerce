using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<EmailAddress>())
               .HasMaxLength(320);

        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(u => u.VerificationStatus)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Ignore(x => x.FullName);

        builder.Property(x => x.Phone).HasMaxLength(30);

        builder.Property(x => x.Role)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.UpdatedAt);

        // -----------------------
        // Backing fields access
        // -----------------------
        builder.Navigation(x => x.OAuthAccounts)
       .HasField("_oauthAccounts")
       .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.RefreshTokens)
               .HasField("_refreshTokens")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Carts)
               .HasField("_carts")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Orders)
               .HasField("_orders")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Favorites)
               .HasField("_favorites")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.StockAlerts)
               .HasField("_stockAlerts")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Notifications)
               .HasField("_notifications")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.AuditLogsAsActor)
               .HasField("_auditLogsAsActor")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.RecoveryCodes)
       .HasField("_recoveryCodes")
       .UsePropertyAccessMode(PropertyAccessMode.Field);

        // -----------------------
        // Relationships
        // -----------------------

        builder.HasMany(x => x.OAuthAccounts)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.RefreshTokens)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Carts)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Orders)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Favorites)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StockAlerts)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Notifications)
               .WithOne(x => x.User)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AuditLogsAsActor)
               .WithOne(x => x.ActorUser)
               .HasForeignKey(x => x.ActorUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}