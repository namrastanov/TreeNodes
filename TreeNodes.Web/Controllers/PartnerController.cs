using MediatR;
using Microsoft.AspNetCore.Mvc;
using TreeNodes.Application.Common.DTOs;

namespace TreeNodes.Web.Controllers;

[ApiController]
[Route("")]
public class PartnerController : ControllerBase
{
    /// <summary>
    /// (Optional) Saves user by unique code and returns auth token.
    /// Stub implementation returning static token.
    /// </summary>
    [HttpPost("api.user.partner.rememberMe")]
    public ActionResult<TokenInfoDto> RememberMe([FromQuery] string code)
    {
        // No actual auth per requirements; return dummy token
        return Ok(new TokenInfoDto { Token = $"token-{code}" });
    }
}


