namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Filter input for journal range.
/// </summary>
public class JournalFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Search { get; set; }
}


