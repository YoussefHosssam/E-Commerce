using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired();

        builder.Property(x => x.Provider)
               .IsRequired()
               .HasMaxLength(30);

        builder.Property(x => x.ProviderPaymentId).HasMaxLength(120);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.Property(x => x.Currency)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<CurrencyCode>())
               .HasMaxLength(3);

        builder.Property(x => x.RawPayloadJson)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.UpdatedAt);

        builder.Metadata.FindNavigation(nameof(Payment.Refunds))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Refunds)
               .WithOne()
               .HasForeignKey(r => r.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.OrderId, x.Status });
        builder.HasIndex(x => x.ProviderPaymentId);
    }
}