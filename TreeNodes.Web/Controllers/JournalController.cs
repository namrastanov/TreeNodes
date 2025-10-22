using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.ExceptionJournal.Queries;

namespace TreeNodes.Web.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class JournalController : ControllerBase
{
    private readonly IMediator _mediator;

    public JournalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Provides the pagination API for exception journal.
    /// </summary>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Number of items to take.</param>
    /// <param name="filter">Optional filter.</param>
    /// <returns>Paginated journal range.</returns>
    [HttpGet("api/journal")]
    public async Task<ActionResult<JournalRangeDto>> GetJournal([FromQuery] int skip, [FromQuery] int take, [FromQuery] JournalFilterDto? filter)
    {
        var result = await _mediator.Send(new GetJournalRangeQuery(skip, take, filter));
        return Ok(result);
    }

    /// <summary>
    /// Returns info about a particular event by ID.
    /// </summary>
    /// <param name="id">Journal record id.</param>
    /// <returns>Single journal record.</returns>
    [HttpGet("api/journal/{id}")]
    public async Task<ActionResult<JournalDto>> GetJournalById([FromRoute] long id)
    {
        var result = await _mediator.Send(new GetJournalSingleQuery(id));
        return Ok(result);
    }
}


