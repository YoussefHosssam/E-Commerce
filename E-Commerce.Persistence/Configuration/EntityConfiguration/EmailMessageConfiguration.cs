// Infrastructure/Persistence/Configurations/EmailMessageConfiguration.cs

using E_Commerce.Domain.Entities;
using E_Commerce.Domain.ValueObjects;
using E_Commerce.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class EmailMessageConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.ToTable("EmailMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MessageType)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(x => x.Subject)
               .IsRequired()
               .HasMaxLength(300);

        builder.Property(x => x.Recipient)
               .IsRequired()
               .HasConversion(ValueConverters.StructString<EmailAddress>())
               .HasMaxLength(320);
        builder.Property(x => x.Sender)
            .IsRequired()
            .HasConversion(ValueConverters.StructString<EmailAddress>())
            .HasMaxLength(320);

        builder.Property(x => x.BodyHtml)
               .IsRequired()
               .HasMaxLength(20000);

        builder.Property(x => x.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(x => x.ProviderName)
               .HasMaxLength(100);

        builder.Property(x => x.Attempts)
               .IsRequired();

        builder.Property(x => x.LastError)
               .HasMaxLength(4000);

        builder.Property(x => x.SentAt);
        builder.Property(x => x.LastAttemptAt);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.RelatedTokenId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Status, x.MessageType });

        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.RelatedToken)
               .WithMany()
               .HasForeignKey(x => x.RelatedTokenId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}