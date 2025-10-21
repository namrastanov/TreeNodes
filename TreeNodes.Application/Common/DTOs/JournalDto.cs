namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Journal view model.
/// </summary>
public class JournalDto
{
    public long Id { get; set; }
    public long EventId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}


