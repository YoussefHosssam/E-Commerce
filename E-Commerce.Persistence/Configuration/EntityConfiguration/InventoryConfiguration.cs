using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(x => x.VariantId);

        builder.Property(x => x.OnHand).IsRequired();
        builder.Property(x => x.Reserved).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasOne(x => x.Variant)
               .WithOne(v => v.Inventory)
               .HasForeignKey<Inventory>(x => x.VariantId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}