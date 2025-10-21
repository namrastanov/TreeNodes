using TreeNodes.Application.Common.DTOs;

namespace TreeNodes.Application.Common.Interfaces;

/// <summary>
/// Abstraction to read journal via queries (optional, kept for future extensions).
/// </summary>
public interface IJournalReader
{
    Task<JournalRangeDto> GetRangeAsync(int skip, int take, JournalFilterDto? filter, CancellationToken cancellationToken);
    Task<JournalDto> GetSingleAsync(long id, CancellationToken cancellationToken);
}


