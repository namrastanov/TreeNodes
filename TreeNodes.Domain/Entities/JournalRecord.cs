using TreeNodes.Domain.Common;

namespace TreeNodes.Domain.Entities;

/// <summary>
/// Exception journal record.
/// </summary>
public class JournalRecord : BaseEntity
{
    public long EventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string RequestPath { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
}


