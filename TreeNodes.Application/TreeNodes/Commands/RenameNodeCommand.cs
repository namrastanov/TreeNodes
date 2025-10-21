using MediatR;

namespace TreeNodes.Application.TreeNodes.Commands;

/// <summary>
/// Rename an existing node.
/// </summary>
public record RenameNodeCommand(long NodeId, string NewNodeName) : IRequest<Unit>;


