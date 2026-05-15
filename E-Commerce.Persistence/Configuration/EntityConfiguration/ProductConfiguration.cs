using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Configurations.OwnedTypes;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
       .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CategoryId).IsRequired();

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Product.Category))!
       .SetPropertyAccessMode(PropertyAccessMode.Property);

        builder.Property(x => x.Slug)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<Slug>())
               .HasMaxLength(150);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.Brand).HasMaxLength(80);

        // Money (BasePrice) — لو Money عندك ValueObject حقيقي: OwnsOne هنا
        builder.OwnsOne(x => x.BasePrice, money =>
        {
            money.WithOwner();
            money.MapMoney("BasePriceAmount", "BasePriceCurrency");
        });

        // لو BasePrice required (عادةً آه):
        builder.Navigation(x => x.BasePrice).IsRequired();

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.Metadata.FindNavigation(nameof(Product.Images))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Images)
               .WithOne(i => i.Product)
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Product.Variants))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Variants)
               .WithOne(v => v.Product)
               .HasForeignKey(v => v.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}