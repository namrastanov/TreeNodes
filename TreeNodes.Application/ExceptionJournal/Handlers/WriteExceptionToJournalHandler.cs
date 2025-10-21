using MediatR;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.ExceptionJournal.Commands;
using TreeNodes.Domain.Entities;

namespace TreeNodes.Application.ExceptionJournal.Handlers;

/// <summary>
/// Handler that writes exception details to the journal and returns the event id.
/// </summary>
public class WriteExceptionToJournalHandler : IRequestHandler<WriteExceptionToJournalCommand, long>
{
    private readonly IAppDbContext _db;

    public WriteExceptionToJournalHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<long> Handle(WriteExceptionToJournalCommand request, CancellationToken cancellationToken)
    {
        var eventId = DateTime.UtcNow.Ticks;
        var entity = new JournalRecord
        {
            EventId = eventId,
            CreatedAt = DateTime.UtcNow,
            RequestPath = request.RequestPath,
            HttpMethod = request.HttpMethod,
            QueryString = request.QueryString,
            Body = request.Body,
            ExceptionType = request.ExceptionType,
            Message = request.Message,
            StackTrace = request.StackTrace
        };

        _db.Journal.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return eventId;
    }
}


