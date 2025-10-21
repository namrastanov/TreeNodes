using MediatR;
using Microsoft.EntityFrameworkCore;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.Common.Interfaces;
using TreeNodes.Application.ExceptionJournal.Queries;

namespace TreeNodes.Application.ExceptionJournal.Handlers;

/// <summary>
/// Handler to fetch paginated journal range.
/// </summary>
public class GetJournalRangeHandler : IRequestHandler<GetJournalRangeQuery, JournalRangeDto>
{
    private readonly IAppDbContext _db;

    public GetJournalRangeHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<JournalRangeDto> Handle(GetJournalRangeQuery request, CancellationToken cancellationToken)
    {
        var q = _db.Journal.AsNoTracking();

        if (request.Filter?.From is not null)
            q = q.Where(x => x.CreatedAt >= request.Filter.From.Value);
        if (request.Filter?.To is not null)
            q = q.Where(x => x.CreatedAt <= request.Filter.To.Value);
        if (!string.IsNullOrWhiteSpace(request.Filter?.Search))
        {
            var s = request.Filter!.Search!;
            q = q.Where(x => x.Message.Contains(s) || x.ExceptionType.Contains(s) || x.RequestPath.Contains(s));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new JournalInfoDto
            {
                Id = x.Id,
                EventId = x.EventId,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new JournalRangeDto
        {
            Skip = request.Skip,
            Count = total,
            Items = items
        };
    }
}


