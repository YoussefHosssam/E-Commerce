using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku).IsRequired().HasMaxLength(64);
        builder.Property(x => x.ProductTitleSnapshot).IsRequired().HasMaxLength(200);

        builder.Property(x => x.VariantSnapshotJson)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.LineTotal).HasPrecision(18, 2);

        builder.Property(x => x.Currency)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<CurrencyCode>())
               .HasMaxLength(3);

        builder.Property(x => x.Quantity).IsRequired();

        builder.HasOne(x => x.Order)
               .WithMany(o => o.Items)
               .HasForeignKey(x => x.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Variant)
               .WithMany()
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.OrderId, x.VariantId });
    }
}