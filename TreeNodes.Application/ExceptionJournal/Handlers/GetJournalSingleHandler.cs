using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.ExceptionJournal.Queries;

namespace TreeNodes.Application.ExceptionJournal.Handlers;

/// <summary>
/// Handler to fetch a single journal record by event id.
/// </summary>
public class GetJournalSingleHandler : IRequestHandler<GetJournalSingleQuery, JournalDto>
{
    private readonly IAppDbContext _db;

    public GetJournalSingleHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<JournalDto> Handle(GetJournalSingleQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Journal.AsNoTracking().FirstOrDefaultAsync(x => x.EventId == request.Id, cancellationToken);
        if (entity is null)
        {
            return new JournalDto();
        }

        return new JournalDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Text = entity.Message,
            CreatedAt = entity.CreatedAt
        };
    }
}


