using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Application.TreeNodes.Commands;
using TreeNodes.Application.TreeNodes.Queries;

namespace TreeNodes.Web.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class TreeController : ControllerBase
{
    private readonly IMediator _mediator;

    public TreeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns entire tree; creates it if not exists.
    /// </summary>
    /// <param name="treeName">Tree name.</param>
    /// <returns>Tree structure.</returns>
    [HttpGet("api/trees/{treeName}")]
    public async Task<ActionResult<NodeDto>> GetTree([FromRoute] string treeName)
    {
        var result = await _mediator.Send(new GetTreeQuery(treeName));
        return Ok(result);
    }

    /// <summary>
    /// Create a new node in a tree.
    /// </summary>
    /// <param name="treeName">Tree name.</param>
    /// <param name="request">Create node request containing parent node id and node name.</param>
    [HttpPost("api/trees/{treeName}/nodes")]
    public async Task<ActionResult> CreateNode([FromRoute] string treeName, [FromBody] CreateNodeRequestDto request)
    {
        var id = await _mediator.Send(new CreateNodeCommand(treeName, request.ParentNodeId, request.NodeName));
        return Ok(new { id });
    }

    /// <summary>
    /// Delete a node and all its descendants.
    /// </summary>
    /// <param name="nodeId">Node id.</param>
    [HttpDelete("api/nodes/{nodeId}")]
    public async Task<ActionResult> DeleteNode([FromRoute] long nodeId)
    {
        await _mediator.Send(new DeleteNodeCommand(nodeId));
        return Ok();
    }

    /// <summary>
    /// Rename node enforcing sibling uniqueness.
    /// </summary>
    /// <param name="nodeId">Node id.</param>
    /// <param name="request">Rename request containing the new node name.</param>
    [HttpPut("api/nodes/{nodeId}")]
    public async Task<ActionResult> RenameNode([FromRoute] long nodeId, [FromBody] RenameNodeRequestDto request)
    {
        await _mediator.Send(new RenameNodeCommand(nodeId, request.NewNodeName));
        return Ok();
    }
}


