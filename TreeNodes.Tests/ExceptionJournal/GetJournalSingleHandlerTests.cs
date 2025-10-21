using FluentAssertions;
using TreeNodes.Application.ExceptionJournal.Handlers;
using TreeNodes.Application.ExceptionJournal.Queries;
using TreeNodes.Domain.Entities;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.ExceptionJournal;

public class GetJournalSingleHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly GetJournalSingleHandler _handler;

    public GetJournalSingleHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new GetJournalSingleHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_WithExistingEventId_ReturnsJournalRecord()
    {
        // Arrange
        var eventId = 12345L;
        var record = new JournalRecord
        {
            EventId = eventId,
            CreatedAt = DateTime.UtcNow,
            Message = "Test exception message",
            ExceptionType = "TestException",
            RequestPath = "/api/test",
            HttpMethod = "GET",
            QueryString = "?param=value",
            Body = "request body",
            StackTrace = "stack trace here"
        };
        _context.Journal.Add(record);
        await _context.SaveChangesAsync();

        var query = new GetJournalSingleQuery(eventId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.EventId.Should().Be(eventId);
        result.Text.Should().Be("Test exception message");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_WithNonExistingEventId_ReturnsEmptyDto()
    {
        // Arrange
        var query = new GetJournalSingleQuery(99999L);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0);
        result.EventId.Should().Be(0);
        result.Text.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithMultipleRecords_ReturnsCorrectOne()
    {
        // Arrange
        var targetEventId = 100L;
        _context.Journal.AddRange(
            new JournalRecord
            {
                EventId = 50L,
                CreatedAt = DateTime.UtcNow,
                Message = "First message",
                ExceptionType = "Exception",
                RequestPath = "/test1",
                HttpMethod = "GET"
            },
            new JournalRecord
            {
                EventId = targetEventId,
                CreatedAt = DateTime.UtcNow,
                Message = "Target message",
                ExceptionType = "Exception",
                RequestPath = "/test2",
                HttpMethod = "POST"
            },
            new JournalRecord
            {
                EventId = 150L,
                CreatedAt = DateTime.UtcNow,
                Message = "Third message",
                ExceptionType = "Exception",
                RequestPath = "/test3",
                HttpMethod = "DELETE"
            }
        );
        await _context.SaveChangesAsync();

        var query = new GetJournalSingleQuery(targetEventId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EventId.Should().Be(targetEventId);
        result.Text.Should().Be("Target message");
    }
}

