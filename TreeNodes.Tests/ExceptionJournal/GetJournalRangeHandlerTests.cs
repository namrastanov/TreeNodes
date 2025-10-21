using FluentAssertions;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.ExceptionJournal.Handlers;
using TreeNodes.Application.ExceptionJournal.Queries;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.ExceptionJournal;

public class GetJournalRangeHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly GetJournalRangeHandler _handler;

    public GetJournalRangeHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new GetJournalRangeHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_WithNoFilter_ReturnsAllRecords()
    {
        // Arrange
        var records = new[]
        {
            new JournalRecord
            {
                EventId = 1,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                Message = "Test message 1",
                ExceptionType = "Exception",
                RequestPath = "/test1",
                HttpMethod = "GET"
            },
            new JournalRecord
            {
                EventId = 2,
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Message = "Test message 2",
                ExceptionType = "Exception",
                RequestPath = "/test2",
                HttpMethod = "POST"
            }
        };
        _context.Journal.AddRange(records);
        await _context.SaveChangesAsync();

        var query = new GetJournalRangeQuery(0, 10, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Skip.Should().Be(0);
        result.Items.Should().BeInDescendingOrder(x => x.CreatedAt);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (int i = 0; i < 5; i++)
        {
            _context.Journal.Add(new JournalRecord
            {
                EventId = i,
                CreatedAt = DateTime.UtcNow.AddHours(-i),
                Message = $"Message {i}",
                ExceptionType = "Exception",
                RequestPath = $"/test{i}",
                HttpMethod = "GET"
            });
        }
        await _context.SaveChangesAsync();

        var query = new GetJournalRangeQuery(2, 2, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
        result.Items.Should().HaveCount(2);
        result.Skip.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithDateFilter_ReturnsFilteredRecords()
    {
        // Arrange
        var baseDate = DateTime.UtcNow;
        _context.Journal.AddRange(
            new JournalRecord
            {
                EventId = 1,
                CreatedAt = baseDate.AddDays(-5),
                Message = "Old message",
                ExceptionType = "Exception",
                RequestPath = "/test",
                HttpMethod = "GET"
            },
            new JournalRecord
            {
                EventId = 2,
                CreatedAt = baseDate.AddDays(-2),
                Message = "Recent message",
                ExceptionType = "Exception",
                RequestPath = "/test",
                HttpMethod = "GET"
            }
        );
        await _context.SaveChangesAsync();

        var filter = new JournalFilterDto
        {
            From = baseDate.AddDays(-3)
        };
        var query = new GetJournalRangeQuery(0, 10, filter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().EventId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WithSearchFilter_ReturnsMatchingRecords()
    {
        // Arrange
        _context.Journal.AddRange(
            new JournalRecord
            {
                EventId = 1,
                CreatedAt = DateTime.UtcNow,
                Message = "Error in user service",
                ExceptionType = "NullReferenceException",
                RequestPath = "/api/users",
                HttpMethod = "GET"
            },
            new JournalRecord
            {
                EventId = 2,
                CreatedAt = DateTime.UtcNow,
                Message = "Database connection failed",
                ExceptionType = "SqlException",
                RequestPath = "/api/products",
                HttpMethod = "POST"
            }
        );
        await _context.SaveChangesAsync();

        var filter = new JournalFilterDto
        {
            Search = "user"
        };
        var query = new GetJournalRangeQuery(0, 10, filter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().EventId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithSearchFilterMatchingExceptionType_ReturnsMatchingRecords()
    {
        // Arrange
        _context.Journal.AddRange(
            new JournalRecord
            {
                EventId = 1,
                CreatedAt = DateTime.UtcNow,
                Message = "Some error",
                ExceptionType = "SqlException",
                RequestPath = "/api/test",
                HttpMethod = "GET"
            },
            new JournalRecord
            {
                EventId = 2,
                CreatedAt = DateTime.UtcNow,
                Message = "Another error",
                ExceptionType = "NullReferenceException",
                RequestPath = "/api/test",
                HttpMethod = "GET"
            }
        );
        await _context.SaveChangesAsync();

        var filter = new JournalFilterDto
        {
            Search = "SqlException"
        };
        var query = new GetJournalRangeQuery(0, 10, filter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Items.First().EventId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyResult()
    {
        // Arrange
        var query = new GetJournalRangeQuery(0, 10, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
        result.Items.Should().BeEmpty();
    }
}

