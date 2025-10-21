using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.TreeNodes.Commands;

namespace TreeNodes.Application.TreeNodes.Handlers;

/// <summary>
/// Handler that renames a node enforcing sibling uniqueness.
/// </summary>
public class RenameNodeHandler : IRequestHandler<RenameNodeCommand, Unit>
{
    private readonly IAppDbContext _db;

    public RenameNodeHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(RenameNodeCommand request, CancellationToken cancellationToken)
    {
        var node = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == request.NodeId, cancellationToken);
        if (node is null)
            throw new SecureException("Node not found");

        var exists = await _db.Nodes.AnyAsync(n => n.TreeId == node.TreeId && n.ParentId == node.ParentId && n.Name == request.NewNodeName && n.Id != node.Id, cancellationToken);
        if (exists)
            throw new NodeNameNotUniqueException(request.NewNodeName);

        node.Name = request.NewNodeName;
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}


