using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.TreeNodes.Commands;
using TreeNodes.Application.TreeNodes.Handlers;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.TreeNodes;

public class RenameNodeHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly RenameNodeHandler _handler;

    public RenameNodeHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new RenameNodeHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_RenamesNodeSuccessfully()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var node = new Node { Name = "OldName", TreeId = tree.Id };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(node.Id, "NewName");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedNode = await _context.Nodes.FindAsync(node.Id);
        updatedNode.Should().NotBeNull();
        updatedNode!.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenNodeNotFound()
    {
        // Arrange
        var command = new RenameNodeCommand(99999, "NewName");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SecureException>()
            .WithMessage("Node not found");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSiblingWithNewNameExists()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var node1 = new Node { Name = "Node1", TreeId = tree.Id };
        var node2 = new Node { Name = "Node2", TreeId = tree.Id };
        _context.Nodes.AddRange(node1, node2);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(node1.Id, "Node2");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NodeNameNotUniqueException>()
            .WithMessage("Node name 'Node2' must be unique among siblings");

        var unchangedNode = await _context.Nodes.FindAsync(node1.Id);
        unchangedNode!.Name.Should().Be("Node1");
    }

    [Fact]
    public async Task Handle_AllowsRenamingToSameNameInDifferentParent()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var parent1 = new Node { Name = "Parent1", TreeId = tree.Id };
        var parent2 = new Node { Name = "Parent2", TreeId = tree.Id };
        _context.Nodes.AddRange(parent1, parent2);
        await _context.SaveChangesAsync();

        var child1 = new Node { Name = "Child", TreeId = tree.Id, ParentId = parent1.Id };
        var child2 = new Node { Name = "ChildToRename", TreeId = tree.Id, ParentId = parent2.Id };
        _context.Nodes.AddRange(child1, child2);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(child2.Id, "Child");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var renamedNode = await _context.Nodes.FindAsync(child2.Id);
        renamedNode!.Name.Should().Be("Child");

        var nodes = await _context.Nodes.Where(n => n.Name == "Child").ToListAsync();
        nodes.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_AllowsRenamingToSameName()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var node = new Node { Name = "CurrentName", TreeId = tree.Id };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(node.Id, "CurrentName");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedNode = await _context.Nodes.FindAsync(node.Id);
        updatedNode!.Name.Should().Be("CurrentName");
    }

    [Fact]
    public async Task Handle_ChecksSiblingUniquenessForChildNodes()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var parent = new Node { Name = "Parent", TreeId = tree.Id };
        _context.Nodes.Add(parent);
        await _context.SaveChangesAsync();

        var child1 = new Node { Name = "Child1", TreeId = tree.Id, ParentId = parent.Id };
        var child2 = new Node { Name = "Child2", TreeId = tree.Id, ParentId = parent.Id };
        _context.Nodes.AddRange(child1, child2);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(child1.Id, "Child2");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NodeNameNotUniqueException>();
    }

    [Fact]
    public async Task Handle_AllowsRenamingWhenNoSiblingsExist()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var parent = new Node { Name = "Parent", TreeId = tree.Id };
        _context.Nodes.Add(parent);
        await _context.SaveChangesAsync();

        var child = new Node { Name = "OnlyChild", TreeId = tree.Id, ParentId = parent.Id };
        _context.Nodes.Add(child);
        await _context.SaveChangesAsync();

        var command = new RenameNodeCommand(child.Id, "RenamedChild");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var renamedNode = await _context.Nodes.FindAsync(child.Id);
        renamedNode!.Name.Should().Be("RenamedChild");
    }
}

