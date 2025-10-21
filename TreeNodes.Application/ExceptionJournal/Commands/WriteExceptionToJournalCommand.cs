using MediatR;

namespace TreeNodes.Application.ExceptionJournal.Commands;

/// <summary>
/// Command to persist exception details into the journal and return the event id.
/// </summary>
public record WriteExceptionToJournalCommand(
    string RequestPath,
    string HttpMethod,
    string QueryString,
    string Body,
    string ExceptionType,
    string Message,
    string StackTrace
) : IRequest<long>;


