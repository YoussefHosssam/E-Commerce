using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VariantImageConfiguration : IEntityTypeConfiguration<VariantImage>
{
    public void Configure(EntityTypeBuilder<VariantImage> builder)
    {
        builder.ToTable("VariantImages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();

        builder.HasOne<Variant>()
               .WithMany(v => v.Images)
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.VariantId, x.SortOrder });
    }
}