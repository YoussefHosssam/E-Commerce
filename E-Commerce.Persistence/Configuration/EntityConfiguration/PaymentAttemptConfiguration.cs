using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PaymentAttemptConfiguration : IEntityTypeConfiguration<PaymentAttempt>
{
    public void Configure(EntityTypeBuilder<PaymentAttempt> builder)
    {
        builder.ToTable("PaymentAttempts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
               .IsRequired();

        builder.Property(x => x.Provider)
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(x => x.ProviderSessionId)
               .HasMaxLength(120);

        builder.Property(x => x.ProviderPaymentId)
               .HasMaxLength(120);

        builder.Property(x => x.PaymentUrl)
               .HasMaxLength(2000);

        builder.Property(x => x.IdempotencyKey)
               .IsRequired()
               .HasMaxLength(120);

        builder.Property(x => x.RequestHash)
               .HasMaxLength(120);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(40);

        builder.Property(x => x.Amount)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(x => x.Currency)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<CurrencyCode>())
               .HasMaxLength(3);

        builder.Property(x => x.RawPayloadJson)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.HasIndex(x => new { x.OrderId, x.Status });

        builder.HasIndex(x => new { x.OrderId, x.IdempotencyKey })
               .IsUnique();

        builder.HasIndex(x => x.ProviderSessionId);

        builder.HasIndex(x => x.ProviderPaymentId);
    }
}