using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Infrastructure.Database.EntitiesConfigurations;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.ToTable("nodes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.TreeId)
            .IsRequired();

        builder.HasOne(x => x.Tree)
            .WithMany(t => t.Nodes)
            .HasForeignKey(x => x.TreeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.TreeId, x.ParentId, x.Name })
            .IsUnique();
    }
}


