using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.TreeNodes.Commands;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Application.TreeNodes.Handlers;

/// <summary>
/// Handler that creates a node in a tree.
/// </summary>
public class CreateNodeHandler : IRequestHandler<CreateNodeCommand, long>
{
    private readonly IAppDbContext _db;

    public CreateNodeHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<long> Handle(CreateNodeCommand request, CancellationToken cancellationToken)
    {
        var tree = await _db.Trees.FirstOrDefaultAsync(t => t.Name == request.TreeName, cancellationToken);
        if (tree is null)
        {
            tree = new Tree { Name = request.TreeName };
            _db.Trees.Add(tree);
            await _db.SaveChangesAsync(cancellationToken);
        }

        Domain.Entities.Node? parent = null;
        if (request.ParentNodeId is not null)
        {
            parent = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == request.ParentNodeId && n.TreeId == tree.Id, cancellationToken);
            if (parent is null)
                throw new SecureException("Parent node not found in the specified tree");
        }

        var siblingExists = await _db.Nodes.AnyAsync(n => n.TreeId == tree.Id && n.ParentId == request.ParentNodeId && n.Name == request.NodeName, cancellationToken);
        if (siblingExists)
            throw new NodeNameNotUniqueException(request.NodeName);

        var node = new Domain.Entities.Node
        {
            Name = request.NodeName,
            TreeId = tree.Id,
            ParentId = parent?.Id
        };

        _db.Nodes.Add(node);
        await _db.SaveChangesAsync(cancellationToken);
        return node.Id;
    }
}


