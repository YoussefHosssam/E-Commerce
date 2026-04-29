using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.AddedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.Cart)
               .WithMany(c => c.Items)
               .HasForeignKey(x => x.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Variant)
               .WithMany()
               .HasForeignKey(x => x.VariantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CartId, x.VariantId }).IsUnique();
    }
}