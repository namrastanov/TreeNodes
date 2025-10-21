using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Infrastructure.Database.EntitiesConfigurations;

public class JournalRecordConfiguration : IEntityTypeConfiguration<JournalRecord>
{
    public void Configure(EntityTypeBuilder<JournalRecord> builder)
    {
        builder.ToTable("journal");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.EventId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.RequestPath)
            .HasMaxLength(2048);

        builder.Property(x => x.HttpMethod)
            .HasMaxLength(16);

        builder.Property(x => x.QueryString)
            .HasColumnType("text");

        builder.Property(x => x.Body)
            .HasColumnType("text");

        builder.Property(x => x.ExceptionType)
            .HasMaxLength(512);

        builder.Property(x => x.Message)
            .HasColumnType("text");

        builder.Property(x => x.StackTrace)
            .HasColumnType("text");

        builder.HasIndex(x => x.EventId)
            .IsUnique();
    }
}


