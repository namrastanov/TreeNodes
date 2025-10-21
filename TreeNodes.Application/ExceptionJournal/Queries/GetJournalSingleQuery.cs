using MediatR;
using TreeNodes.Application.Common.DTOs;

namespace TreeNodes.Application.ExceptionJournal.Queries;

/// <summary>
/// Query to get a single journal record by event id.
/// </summary>
public record GetJournalSingleQuery(long Id) : IRequest<JournalDto>;


