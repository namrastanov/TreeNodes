using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.TreeNodes.Commands;
using TreeNodes.Application.TreeNodes.Handlers;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.TreeNodes;

public class DeleteNodeHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly DeleteNodeHandler _handler;

    public DeleteNodeHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new DeleteNodeHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_DeletesNodeWithoutChildren()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var node = new Node { Name = "NodeToDelete", TreeId = tree.Id };
        _context.Nodes.Add(node);
        await _context.SaveChangesAsync();

        var command = new DeleteNodeCommand(node.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedNode = await _context.Nodes.FindAsync(node.Id);
        deletedNode.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenNodeHasChildren()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var parent = new Node { Name = "Parent", TreeId = tree.Id };
        _context.Nodes.Add(parent);
        await _context.SaveChangesAsync();

        var child = new Node { Name = "Child", TreeId = tree.Id, ParentId = parent.Id };
        _context.Nodes.Add(child);
        await _context.SaveChangesAsync();

        var command = new DeleteNodeCommand(parent.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DeleteChildrenFirstException>()
            .WithMessage("You have to delete all children nodes first");

        var parentStillExists = await _context.Nodes.FindAsync(parent.Id);
        parentStillExists.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ReturnsUnit_WhenNodeDoesNotExist()
    {
        // Arrange
        var command = new DeleteNodeCommand(99999);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
    }

    [Fact]
    public async Task Handle_DeletesLeafNodeWithSiblings()
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

        var command = new DeleteNodeCommand(child1.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedNode = await _context.Nodes.FindAsync(child1.Id);
        deletedNode.Should().BeNull();

        var remainingChild = await _context.Nodes.FindAsync(child2.Id);
        remainingChild.Should().NotBeNull();

        var parentNode = await _context.Nodes.FindAsync(parent.Id);
        parentNode.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenNodeHasMultipleChildren()
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
        var child3 = new Node { Name = "Child3", TreeId = tree.Id, ParentId = parent.Id };
        _context.Nodes.AddRange(child1, child2, child3);
        await _context.SaveChangesAsync();

        var command = new DeleteNodeCommand(parent.Id);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DeleteChildrenFirstException>();

        var children = await _context.Nodes.Where(n => n.ParentId == parent.Id).ToListAsync();
        children.Should().HaveCount(3);
    }
}

