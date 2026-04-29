using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class UserOAuthAccountConfiguration : IEntityTypeConfiguration<UserOAuthAccount>
{
    public void Configure(EntityTypeBuilder<UserOAuthAccount> builder)
    {
        builder.ToTable("UserOAuthAccounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Provider)
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(x => x.ProviderUserId)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Email)
               .HasMaxLength(254);

        builder.Property(x => x.LinkedAt)
               .IsRequired();

        builder.HasOne(x => x.User)
               .WithMany(u => u.OAuthAccounts)   // لازم يكون عندك ICollection<UserOAuthAccount> في User
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        // Unique: نفس اليوزر مينفعش يكرر نفس provider+providerUserId
        builder.HasIndex(x => new { x.Provider, x.ProviderUserId })
               .IsUnique();

        builder.HasIndex(x => x.UserId);
    }
}