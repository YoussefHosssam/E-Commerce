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

        builder.Property(x => x.ProductId).IsRequired();

        builder.Property(x => x.Sku)
               .IsRequired()
               .HasMaxLength(64);

        builder.Property(x => x.Size).HasMaxLength(30);
        builder.Property(x => x.Color).HasMaxLength(30);

        builder.Property(x => x.IsActive).IsRequired();

        // Optional Money override
        builder.OwnsOne(x => x.PriceOverride, money =>
        {
            money.WithOwner();

            money.Property(m => m.Amount)
                 .HasPrecision(18, 2)
                 .HasColumnName("PriceOverrideAmount");

            money.Property(m => m.Currency)
                 .HasConversion(ValueConverters.StructString<CurrencyCode>())
                 .HasMaxLength(3)
                 .HasColumnName("PriceOverrideCurrency");
        });

        // PriceOverride nullable
        builder.Navigation(x => x.PriceOverride).IsRequired(false);

        builder.HasIndex(x => x.Sku).IsUnique(); // global unique (حسب ما كتبت في الدومين)
        builder.HasIndex(x => new { x.ProductId, x.Sku }).IsUnique();

        builder.Metadata.FindNavigation(nameof(Variant.Images))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany<VariantImage>()
               .WithOne()
               .HasForeignKey(i => i.VariantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}