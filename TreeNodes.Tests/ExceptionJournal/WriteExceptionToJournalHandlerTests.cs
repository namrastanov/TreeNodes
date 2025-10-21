using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.ExceptionJournal.Commands;
using TreeNodes.Application.ExceptionJournal.Handlers;
using TreeNodes.Tests.Helpers;

namespace TreeNodes.Tests.ExceptionJournal;

public class WriteExceptionToJournalHandlerTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly WriteExceptionToJournalHandler _handler;

    public WriteExceptionToJournalHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new WriteExceptionToJournalHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_CreatesJournalRecordAndReturnsEventId()
    {
        // Arrange
        var command = new WriteExceptionToJournalCommand(
            RequestPath: "/api/test",
            HttpMethod: "POST",
            QueryString: "?id=123",
            Body: "{\"test\": \"data\"}",
            ExceptionType: "NullReferenceException",
            Message: "Object reference not set",
            StackTrace: "at SomeMethod()"
        );

        // Act
        var eventId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        eventId.Should().BeGreaterThan(0);

        var savedRecord = await _context.Journal.FirstOrDefaultAsync(j => j.EventId == eventId);
        savedRecord.Should().NotBeNull();
        savedRecord!.RequestPath.Should().Be("/api/test");
        savedRecord.HttpMethod.Should().Be("POST");
        savedRecord.QueryString.Should().Be("?id=123");
        savedRecord.Body.Should().Be("{\"test\": \"data\"}");
        savedRecord.ExceptionType.Should().Be("NullReferenceException");
        savedRecord.Message.Should().Be("Object reference not set");
        savedRecord.StackTrace.Should().Be("at SomeMethod()");
        savedRecord.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_GeneratesUniqueEventIds()
    {
        // Arrange
        var command1 = new WriteExceptionToJournalCommand(
            RequestPath: "/api/test1",
            HttpMethod: "GET",
            QueryString: "",
            Body: "",
            ExceptionType: "Exception1",
            Message: "Message 1",
            StackTrace: "Stack 1"
        );

        var command2 = new WriteExceptionToJournalCommand(
            RequestPath: "/api/test2",
            HttpMethod: "POST",
            QueryString: "",
            Body: "",
            ExceptionType: "Exception2",
            Message: "Message 2",
            StackTrace: "Stack 2"
        );

        // Act
        var eventId1 = await _handler.Handle(command1, CancellationToken.None);
        await Task.Delay(10); // Ensure different timestamps
        var eventId2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        eventId1.Should().NotBe(eventId2);
        
        var records = await _context.Journal.ToListAsync();
        records.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithMinimalData_CreatesRecord()
    {
        // Arrange
        var command = new WriteExceptionToJournalCommand(
            RequestPath: "/",
            HttpMethod: "GET",
            QueryString: "",
            Body: "",
            ExceptionType: "Exception",
            Message: "Error",
            StackTrace: ""
        );

        // Act
        var eventId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        eventId.Should().BeGreaterThan(0);

        var savedRecord = await _context.Journal.FirstOrDefaultAsync(j => j.EventId == eventId);
        savedRecord.Should().NotBeNull();
        savedRecord!.RequestPath.Should().Be("/");
        savedRecord.HttpMethod.Should().Be("GET");
        savedRecord.ExceptionType.Should().Be("Exception");
        savedRecord.Message.Should().Be("Error");
    }
}

