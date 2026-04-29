using E_Commerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).IsRequired().HasMaxLength(80);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Body).IsRequired().HasMaxLength(2000);

        builder.Property(x => x.DataJson)
               .IsRequired()
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.ReadAt);

        builder.HasOne(x => x.User)
               .WithMany(u => u.Notifications)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.IsRead });
    }
}