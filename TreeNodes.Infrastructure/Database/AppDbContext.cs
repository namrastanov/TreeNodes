using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Infrastructure.Database;

/// <summary>
/// EF Core DbContext for the application.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tree> Trees => Set<Tree>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<JournalRecord> Journal => Set<JournalRecord>();

    /// <summary>
    /// Configure model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}


