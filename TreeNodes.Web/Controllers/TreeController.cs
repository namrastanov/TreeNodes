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
    [HttpPost("api.user.tree.get")]
    public async Task<ActionResult<NodeDto>> GetTree([FromQuery] string treeName)
    {
        var result = await _mediator.Send(new GetTreeQuery(treeName));
        return Ok(result);
    }

    /// <summary>
    /// Create a new node in a tree.
    /// </summary>
    /// <param name="treeName">Tree name.</param>
    /// <param name="parentNodeId">Optional parent node id.</param>
    /// <param name="nodeName">Node name.</param>
    [HttpPost("api.user.tree.node.create")]
    public async Task<ActionResult> Create([FromQuery] string treeName, [FromQuery] long? parentNodeId, [FromQuery] string nodeName)
    {
        var id = await _mediator.Send(new CreateNodeCommand(treeName, parentNodeId, nodeName));
        return Ok(new { id });
    }

    /// <summary>
    /// Delete a node and all its descendants.
    /// </summary>
    /// <param name="nodeId">Node id.</param>
    [HttpPost("api.user.tree.node.delete")]
    public async Task<ActionResult> Delete([FromQuery] long nodeId)
    {
        await _mediator.Send(new DeleteNodeCommand(nodeId));
        return Ok();
    }

    /// <summary>
    /// Rename node enforcing sibling uniqueness.
    /// </summary>
    /// <param name="nodeId">Node id.</param>
    /// <param name="newNodeName">New node name.</param>
    [HttpPost("api.user.tree.node.rename")]
    public async Task<ActionResult> Rename([FromQuery] long nodeId, [FromQuery] string newNodeName)
    {
        await _mediator.Send(new RenameNodeCommand(nodeId, newNodeName));
        return Ok();
    }
}


