using MediatR;

namespace TreeNodes.Application.TreeNodes.Commands;

/// <summary>
/// Create a new node within a tree.
/// </summary>
public record CreateNodeCommand(string TreeName, long? ParentNodeId, string NodeName) : IRequest<long>;


