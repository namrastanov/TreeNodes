using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.TreeNodes.Commands;

namespace TreeNodes.Application.TreeNodes.Handlers;

/// <summary>
/// Handler that deletes a node and its descendants.
/// </summary>
public class DeleteNodeHandler : IRequestHandler<DeleteNodeCommand, Unit>
{
    private readonly IAppDbContext _db;

    public DeleteNodeHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteNodeCommand request, CancellationToken cancellationToken)
    {
        var node = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == request.NodeId, cancellationToken);
        if (node is null)
            return Unit.Value;

        var hasChildren = await _db.Nodes.AnyAsync(n => n.ParentId == node.Id, cancellationToken);
        if (hasChildren)
            throw new DeleteChildrenFirstException();

        _db.Nodes.Remove(node);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}


