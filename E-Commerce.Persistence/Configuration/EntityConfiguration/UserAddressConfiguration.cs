using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Persistence.Configuration.EntityConfiguration;

internal sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Label).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Governorate).IsRequired().HasMaxLength(100);
        builder.Property(x => x.City).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Area).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Street).IsRequired().HasMaxLength(200);
        builder.Property(x => x.BuildingNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Floor).HasMaxLength(50);
        builder.Property(x => x.Apartment).HasMaxLength(50);
        builder.Property(x => x.PostalCode).HasMaxLength(30);
        builder.Property(x => x.Landmark).HasMaxLength(300);
        builder.Property(x => x.Latitude).HasPrecision(9, 6);
        builder.Property(x => x.Longitude).HasPrecision(9, 6);
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.IsDefault })
            .IsUnique()
            .HasFilter("[IsDefault] = 1");
    }
}
