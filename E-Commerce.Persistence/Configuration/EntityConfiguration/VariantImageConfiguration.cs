using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class VariantImageConfiguration : IEntityTypeConfiguration<VariantImage>
{
    public void Configure(EntityTypeBuilder<VariantImage> builder)
    {
        builder.ToTable("VariantImages");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StorageKey).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.Width).IsRequired();
        builder.Property(x => x.Height).IsRequired();
        builder.Property(x => x.SizeInBytes).IsRequired();
        builder.Property(x => x.Format).IsRequired().HasMaxLength(20);
        builder.Property(x => x.IsPrimary).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.ProcessingStatus).IsRequired().HasConversion<string>().HasMaxLength(30);

        builder.HasOne<Variant>()
               .WithMany(v => v.Images)
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.VariantId);
        builder.HasIndex(x => x.StorageKey).IsUnique();
        builder.HasIndex(x => new { x.VariantId, x.SortOrder });
    }
}
