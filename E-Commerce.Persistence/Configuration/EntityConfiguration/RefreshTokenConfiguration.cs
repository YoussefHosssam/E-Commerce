using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.TokenHash)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<TokenHash>())
               .HasMaxLength(256);

        builder.Property(x => x.ReplacedByTokenHash)
               .HasConversion(ValueConverters.StructString<TokenHash>())
               .HasMaxLength(256);

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.RevokedAt);

        builder.Property(x => x.DeviceInfo).HasMaxLength(300);
        builder.Property(x => x.IpAddress).HasMaxLength(45);

        builder.HasOne(x => x.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}