using TreeNodes.Domain.Common;

namespace TreeNodes.Domain.Entities;

/// <summary>
/// Represents a node in a tree hierarchy.
/// </summary>
public class Node : BaseEntity
{
    /// <summary>
    /// Node name. Must be unique among siblings within the same tree.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to the owning tree.
    /// </summary>
    public long TreeId { get; set; }

    /// <summary>
    /// The tree entity.
    /// </summary>
    public Tree? Tree { get; set; }

    /// <summary>
    /// Optional parent node reference; null for root nodes.
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// Parent node navigation.
    /// </summary>
    public Node? Parent { get; set; }

    /// <summary>
    /// Child nodes within the same tree.
    /// </summary>
    public ICollection<Node> Children { get; set; } = new List<Node>();
}


