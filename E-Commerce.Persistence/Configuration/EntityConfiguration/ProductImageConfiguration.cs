using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();

        builder.HasOne(x => x.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.ProductId, x.SortOrder });
    }
}