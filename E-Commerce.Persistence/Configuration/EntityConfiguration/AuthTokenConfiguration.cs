// Infrastructure/Persistence/Configurations/AuthTokenConfiguration.cs

using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.ToTable("AuthTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
               .IsRequired();

        builder.Property(x => x.TokenHash)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<TokenHash>())
               .HasMaxLength(500);

        builder.Property(x => x.TokenType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.ConsumedAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => new { x.UserId, x.TokenType });
        builder.HasIndex(x => x.TokenHash)
               .IsUnique();

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}