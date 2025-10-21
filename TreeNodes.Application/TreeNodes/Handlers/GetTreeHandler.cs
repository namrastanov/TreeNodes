using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.TreeNodes.Queries;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Application.TreeNodes.Handlers;

/// <summary>
/// Handler that returns full tree and creates it if missing.
/// </summary>
public class GetTreeHandler : IRequestHandler<GetTreeQuery, NodeDto>
{
    private readonly IAppDbContext _db;

    public GetTreeHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<NodeDto> Handle(GetTreeQuery request, CancellationToken cancellationToken)
    {
        var tree = await _db.Trees.FirstOrDefaultAsync(t => t.Name == request.TreeName, cancellationToken);
        if (tree is null)
        {
            tree = new Tree { Name = request.TreeName };
            _db.Trees.Add(tree);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var roots = await _db.Nodes.AsNoTracking()
            .Where(n => n.TreeId == tree.Id && n.ParentId == null)
            .ToListAsync(cancellationToken);

        var dtoRoot = new NodeDto
        {
            Id = tree.Id,
            Name = tree.Name,
            Children = new List<NodeDto>()
        };

        foreach (var root in roots)
        {
            dtoRoot.Children.Add(await BuildDtoAsync(root, cancellationToken));
        }

        return dtoRoot;
    }

    private async Task<NodeDto> BuildDtoAsync(Domain.Entities.Node node, CancellationToken cancellationToken)
    {
        var dto = new NodeDto { Id = node.Id, Name = node.Name };
        var children = await _db.Nodes.AsNoTracking()
            .Where(n => n.ParentId == node.Id)
            .ToListAsync(cancellationToken);
        foreach (var child in children)
        {
            dto.Children.Add(await BuildDtoAsync(child, cancellationToken));
        }
        return dto;
    }
}


