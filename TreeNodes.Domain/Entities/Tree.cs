using TreeNodes.Domain.Common;

namespace TreeNodes.Domain.Entities;

/// <summary>
/// Represents a named tree which owns a hierarchy of nodes.
/// </summary>
public class Tree : BaseEntity
{
    /// <summary>
    /// Human-readable unique name of the tree.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nodes that belong to this tree.
    /// </summary>
    public ICollection<Node> Nodes { get; set; } = new List<Node>();
}


