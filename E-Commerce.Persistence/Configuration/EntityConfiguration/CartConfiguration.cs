using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.AnonymousToken).HasMaxLength(128);
        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Carts)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Backing field collection
        builder.Metadata.FindNavigation(nameof(Cart.Items))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
               .WithOne(i => i.Cart)
               .HasForeignKey(i => i.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AnonymousToken)
               .IsUnique()
               .HasFilter("[AnonymousToken] IS NOT NULL");

        builder.HasIndex(x => x.UserId)
               .HasFilter("[UserId] IS NOT NULL");
    }
}