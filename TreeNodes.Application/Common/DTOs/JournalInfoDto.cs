namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Paginated journal item info.
/// </summary>
public class JournalInfoDto
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; }
}


