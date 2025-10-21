using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.TreeNodes.Handlers;
using TreeNodes.Application.TreeNodes.Queries;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.TreeNodes;

public class GetTreeHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly GetTreeHandler _handler;

    public GetTreeHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new GetTreeHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_CreatesTreeIfNotExists()
    {
        // Arrange
        var query = new GetTreeQuery("NewTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("NewTree");
        result.Children.Should().BeEmpty();

        var tree = await _context.Trees.FirstOrDefaultAsync(t => t.Name == "NewTree");
        tree.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ReturnsExistingEmptyTree()
    {
        // Arrange
        var tree = new Tree { Name = "ExistingTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("ExistingTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tree.Id);
        result.Name.Should().Be("ExistingTree");
        result.Children.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsTreeWithSingleRootNode()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var rootNode = new Node { Name = "Root", TreeId = tree.Id };
        _context.Nodes.Add(rootNode);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("TestTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestTree");
        result.Children.Should().HaveCount(1);
        result.Children.First().Name.Should().Be("Root");
        result.Children.First().Children.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsTreeWithMultipleRootNodes()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var root1 = new Node { Name = "Root1", TreeId = tree.Id };
        var root2 = new Node { Name = "Root2", TreeId = tree.Id };
        _context.Nodes.AddRange(root1, root2);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("TestTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Children.Should().HaveCount(2);
        result.Children.Should().Contain(n => n.Name == "Root1");
        result.Children.Should().Contain(n => n.Name == "Root2");
    }

    [Fact]
    public async Task Handle_ReturnsTreeWithNestedStructure()
    {
        // Arrange
        var tree = new Tree { Name = "TestTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        var root = new Node { Name = "Root", TreeId = tree.Id };
        _context.Nodes.Add(root);
        await _context.SaveChangesAsync();

        var child1 = new Node { Name = "Child1", TreeId = tree.Id, ParentId = root.Id };
        var child2 = new Node { Name = "Child2", TreeId = tree.Id, ParentId = root.Id };
        _context.Nodes.AddRange(child1, child2);
        await _context.SaveChangesAsync();

        var grandChild = new Node { Name = "GrandChild", TreeId = tree.Id, ParentId = child1.Id };
        _context.Nodes.Add(grandChild);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("TestTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Children.Should().HaveCount(1);

        var rootDto = result.Children.First();
        rootDto.Name.Should().Be("Root");
        rootDto.Children.Should().HaveCount(2);

        var child1Dto = rootDto.Children.First(c => c.Name == "Child1");
        child1Dto.Children.Should().HaveCount(1);
        child1Dto.Children.First().Name.Should().Be("GrandChild");

        var child2Dto = rootDto.Children.First(c => c.Name == "Child2");
        child2Dto.Children.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ReturnsComplexTreeStructure()
    {
        // Arrange
        var tree = new Tree { Name = "ComplexTree" };
        _context.Trees.Add(tree);
        await _context.SaveChangesAsync();

        // Create multiple root nodes with various depths
        var root1 = new Node { Name = "Root1", TreeId = tree.Id };
        var root2 = new Node { Name = "Root2", TreeId = tree.Id };
        _context.Nodes.AddRange(root1, root2);
        await _context.SaveChangesAsync();

        var r1Child1 = new Node { Name = "R1-Child1", TreeId = tree.Id, ParentId = root1.Id };
        var r1Child2 = new Node { Name = "R1-Child2", TreeId = tree.Id, ParentId = root1.Id };
        var r2Child1 = new Node { Name = "R2-Child1", TreeId = tree.Id, ParentId = root2.Id };
        _context.Nodes.AddRange(r1Child1, r1Child2, r2Child1);
        await _context.SaveChangesAsync();

        var r1c1GrandChild = new Node { Name = "R1-C1-GC", TreeId = tree.Id, ParentId = r1Child1.Id };
        _context.Nodes.Add(r1c1GrandChild);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("ComplexTree");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Children.Should().HaveCount(2);

        var root1Dto = result.Children.First(r => r.Name == "Root1");
        root1Dto.Children.Should().HaveCount(2);
        root1Dto.Children.First(c => c.Name == "R1-Child1").Children.Should().HaveCount(1);
        root1Dto.Children.First(c => c.Name == "R1-Child2").Children.Should().BeEmpty();

        var root2Dto = result.Children.First(r => r.Name == "Root2");
        root2Dto.Children.Should().HaveCount(1);
        root2Dto.Children.First().Name.Should().Be("R2-Child1");
    }

    [Fact]
    public async Task Handle_OnlyReturnsNodesFromSpecifiedTree()
    {
        // Arrange
        var tree1 = new Tree { Name = "Tree1" };
        var tree2 = new Tree { Name = "Tree2" };
        _context.Trees.AddRange(tree1, tree2);
        await _context.SaveChangesAsync();

        var tree1Node = new Node { Name = "Tree1Node", TreeId = tree1.Id };
        var tree2Node = new Node { Name = "Tree2Node", TreeId = tree2.Id };
        _context.Nodes.AddRange(tree1Node, tree2Node);
        await _context.SaveChangesAsync();

        var query = new GetTreeQuery("Tree1");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Children.Should().HaveCount(1);
        result.Children.First().Name.Should().Be("Tree1Node");
    }
}

