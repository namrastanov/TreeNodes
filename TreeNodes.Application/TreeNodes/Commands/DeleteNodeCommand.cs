using MediatR;

namespace TreeNodes.Application.TreeNodes.Commands;

/// <summary>
/// Delete a node and all its descendants by id.
/// </summary>
public record DeleteNodeCommand(long NodeId) : IRequest<Unit>;


