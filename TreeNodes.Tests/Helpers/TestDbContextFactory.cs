using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Tests.Helpers;

/// <summary>
/// Factory for creating in-memory test database contexts.
/// </summary>
public class TestDbContext : DbContext, IAppDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<Tree> Trees { get; set; } = null!;
    public DbSet<Node> Nodes { get; set; } = null!;
    public DbSet<JournalRecord> Journal { get; set; } = null!;
}

public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a new in-memory database context for testing.
    /// </summary>
    public static TestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

