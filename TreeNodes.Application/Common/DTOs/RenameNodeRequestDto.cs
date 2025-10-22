namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Request DTO for renaming a node.
/// </summary>
public class RenameNodeRequestDto
{
    /// <summary>
    /// New node name.
    /// </summary>
    public string NewNodeName { get; set; } = string.Empty;
}

