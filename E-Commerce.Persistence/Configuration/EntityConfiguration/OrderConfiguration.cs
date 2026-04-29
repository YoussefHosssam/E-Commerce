using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.OrderNumber)
               .IsRequired()
               .HasMaxLength(40);

        builder.HasIndex(x => x.OrderNumber).IsUnique();

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.Subtotal).HasPrecision(18, 2);
        builder.Property(x => x.ShippingFee).HasPrecision(18, 2);
        builder.Property(x => x.DiscountTotal).HasPrecision(18, 2);
        builder.Property(x => x.TaxTotal).HasPrecision(18, 2);
        builder.Property(x => x.GrandTotal).HasPrecision(18, 2);

        builder.Property(x => x.Currency)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<CurrencyCode>())
               .HasMaxLength(3);

        builder.Property(x => x.ShippingAddressJson)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.BillingAddressJson)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Order.Items))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
               .WithOne(i => i.Order)
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Order.Payments))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Payments)
               .WithOne()
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.Status });
    }
}