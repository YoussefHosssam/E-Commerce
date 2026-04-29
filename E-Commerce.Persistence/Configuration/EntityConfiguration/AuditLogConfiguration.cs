using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActorUserId).IsRequired();

        builder.Property(x => x.Action)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(64);

        builder.Property(x => x.EntityType)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.EntityId)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.OldValuesJson)
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.NewValuesJson)
               .HasConversion(ValueConverters.StructString<JsonText>())
               .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IpAddress).HasMaxLength(64);
        builder.Property(x => x.UserAgent).HasMaxLength(512);

        builder.HasOne(x => x.ActorUser)
               .WithMany(u => u.AuditLogsAsActor)
               .HasForeignKey(x => x.ActorUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ActorUserId);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
    }
}