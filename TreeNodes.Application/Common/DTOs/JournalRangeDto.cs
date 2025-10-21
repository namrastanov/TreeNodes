namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Range page model for journals.
/// </summary>
public class JournalRangeDto
{
    public int Skip { get; set; }
    public int Count { get; set; }
    public IReadOnlyList<JournalInfoDto> Items { get; set; } = Array.Empty<JournalInfoDto>();
}


