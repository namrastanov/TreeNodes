using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.Exceptions;
using TreeNodes.Application.TreeNodes.Commands;
using TreeNodes.Application.TreeNodes.Handlers;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.TreeNodes;

public class CreateNodeHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly CreateNodeHandler _handler;

    public CreateNodeHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new CreateNodeHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_CreatesTreeIfNotExists()
    {
        // Arrange
        var command = new CreateNodeCommand("NewTree", null, "RootNode");

        // Act
        var nodeId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        nodeId.Should().BeGreaterThan(0);

        var tree = await _context.Trees.FirstOrDefaultAsync(t => t.Name == "NewTree");
        tree.Should().NotBeNull();

        var node = await _context.Nodes.FindAsync(nodeId);
        node.Should().NotBeNull();
        node!.Name.Should().Be("RootNode");
        node.TreeId.Should().Be(tree!.Id);
        node.ParentId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_UsesExistingTree()
    {
        // Arrange
        var tree = new Tree { Name = "ExistingTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("ExistingTree", null, "NewNode");

        // Act
        var nodeId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        nodeId.Should().BeGreaterThan(0);

        var trees = await _context.Trees.Where(t => t.Name == "ExistingTree").ToListAsync();
        trees.Should().HaveCount(1);
        trees.First().Id.Should().Be(tree.Id);

        var node = await _context.Nodes.FindAsync(nodeId);
        node.Should().NotBeNull();
        node!.TreeId.Should().Be(tree.Id);
    }

    [Fact]
    public async Task Handle_CreatesChildNode()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var parentNode = new Node { Name = "Parent", TreeId = tree.Id };
        _context.Nodes.Add(parentNode);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("TestTree", parentNode.Id, "Child");

        // Act
        var childId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        childId.Should().BeGreaterThan(0);

        var child = await _context.Nodes.FindAsync(childId);
        child.Should().NotBeNull();
        child!.Name.Should().Be("Child");
        child.ParentId.Should().Be(parentNode.Id);
        child.TreeId.Should().Be(tree.Id);
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParentNodeNotFound()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("TestTree", 99999, "Child");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SecureException>()
            .WithMessage("Parent node not found in the specified tree");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenParentNodeInDifferentTree()
    {
        // Arrange
        var tree1 = new Tree { Name = "Tree1" };
        var tree2 = new Tree { Name = "Tree2" };
        _context.Trees.AddRange(tree1, tree2);
        await _context.SaveChangesAsync();

        var nodeInTree1 = new Node { Name = "Node1", TreeId = tree1.Id };
        _context.Nodes.Add(nodeInTree1);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("Tree2", nodeInTree1.Id, "Child");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<SecureException>()
            .WithMessage("Parent node not found in the specified tree");
    }

    [Fact]
    public async Task Handle_ThrowsException_WhenSiblingWithSameNameExists()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var existingNode = new Node { Name = "DuplicateName", TreeId = tree.Id };
        _context.Nodes.Add(existingNode);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("TestTree", null, "DuplicateName");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NodeNameNotUniqueException>()
            .WithMessage("Node name 'DuplicateName' must be unique among siblings");
    }

    [Fact]
    public async Task Handle_AllowsSameNameInDifferentParents()
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
        _context.Nodes.Add(child1);
        await _context.SaveChangesAsync();

        var command = new CreateNodeCommand("TestTree", parent2.Id, "Child");

        // Act
        var nodeId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        nodeId.Should().BeGreaterThan(0);

        var nodes = await _context.Nodes.Where(n => n.Name == "Child").ToListAsync();
        nodes.Should().HaveCount(2);
    }
}

