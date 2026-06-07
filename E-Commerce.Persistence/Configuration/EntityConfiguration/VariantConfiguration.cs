using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.ToTable("Variants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
       .ValueGeneratedNever();

        builder.Property(x => x.ProductId).IsRequired();

        builder.Property(x => x.Sku)
               .IsRequired()
               .HasMaxLength(64);

        builder.Property(x => x.Size).HasMaxLength(30);
        builder.Property(x => x.IsDefault).IsRequired();

        builder.OwnsOne(x => x.Color, color =>
        {
            color.WithOwner();

            color.Property(c => c.Name)
                 .IsRequired()
                 .HasMaxLength(30)
                 .HasColumnName("ColorName");

            color.Property(c => c.HexCode)
                 .IsRequired()
                 .HasMaxLength(7)
                 .HasColumnName("ColorHexCode");
        });

        builder.Navigation(x => x.Color).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();

        builder.OwnsOne(x => x.Price, money =>
        {
            money.WithOwner();

            money.Property(m => m.Amount)
                 .HasPrecision(18, 2)
                 .HasColumnName("PriceAmount");

            money.Property(m => m.Currency)
                 .HasConversion(ValueConverters.StructString<CurrencyCode>())
                 .HasMaxLength(3)
                 .HasColumnName("PriceCurrency");
        });

        builder.Navigation(x => x.Price).IsRequired(false);

        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.Sku }).IsUnique();
        builder.HasIndex(x => new { x.ProductId, x.IsDefault })
               .IsUnique()
               .HasFilter("[IsDefault] = 1 AND [IsActive] = 1");

        builder.Metadata.FindNavigation(nameof(Variant.Images))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        // VariantConfiguration
        builder.HasMany(v => v.Images)
               .WithOne()
               .HasForeignKey(i => i.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Images)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(x => x.Inventory)
                .WithOne(x => x.Variant)
                .HasForeignKey<Inventory>(x => x.VariantId)
                .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany<StockMovement>()
               .WithOne(x => x.Variant)
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
