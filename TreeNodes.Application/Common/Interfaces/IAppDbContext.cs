using Microsoft.EntityFrameworkCore;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Application.Common.Interfaces;

/// <summary>
/// Abstraction for application database context.
/// </summary>
public interface IAppDbContext
{
    DbSet<Tree> Trees { get; }
    DbSet<Node> Nodes { get; }
    DbSet<JournalRecord> Journal { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}


