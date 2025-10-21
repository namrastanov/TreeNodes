using MediatR;
using TreeNodes.Application.Common.DTOs;

namespace TreeNodes.Application.ExceptionJournal.Queries;

/// <summary>
/// Query to get paginated journal items.
/// </summary>
public record GetJournalRangeQuery(int Skip, int Take, JournalFilterDto? Filter) : IRequest<JournalRangeDto>;


