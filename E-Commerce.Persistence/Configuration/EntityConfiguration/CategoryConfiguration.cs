using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<Slug>())
               .HasMaxLength(150);

        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasOne(x => x.Parent)
               .WithMany(x => x.Children)
               .HasForeignKey(x => x.ParentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Category.Children))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Category.Products))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}