namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Request DTO for creating a new node.
/// </summary>
public class CreateNodeRequestDto
{
    /// <summary>
    /// Optional parent node id.
    /// </summary>
    public long? ParentNodeId { get; set; }

    /// <summary>
    /// Node name.
    /// </summary>
    public string NodeName { get; set; } = string.Empty;
}

