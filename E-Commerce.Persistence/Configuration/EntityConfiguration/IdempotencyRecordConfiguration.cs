using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Persistence.Configuration.EntityConfiguration
{
    using E_Commerce.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId);

            builder.Property(x => x.Operation)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.IdempotencyKey)
                   .IsRequired()
                   .HasMaxLength(120);

            builder.Property(x => x.RequestHash)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(x => x.ResponseStatusCode);

            builder.Property(x => x.ResponseBodyJson)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.ContentType)
                   .HasMaxLength(100);

            builder.Property(x => x.ResourceId)
                   .HasMaxLength(120);

            builder.Property(x => x.FailureReason)
                   .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.CompletedAt);

            builder.Property(x => x.ExpiresAt)
                   .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.Operation, x.IdempotencyKey })
                   .IsUnique();

            builder.HasIndex(x => new { x.Operation, x.IdempotencyKey });

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.ExpiresAt);

            builder.HasIndex(x => x.ResourceId);
        }
    }
}
