using MediatR;
using TreeNodes.Application.Common.DTOs;

namespace TreeNodes.Application.TreeNodes.Queries;

/// <summary>
/// Get or create a tree by name and return its full structure.
/// </summary>
public record GetTreeQuery(string TreeName) : IRequest<NodeDto>;


