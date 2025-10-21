namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Tree node DTO mirroring swagger.
/// </summary>
public class NodeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<NodeDto> Children { get; set; } = new();
}


