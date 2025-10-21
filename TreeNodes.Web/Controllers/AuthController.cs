using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreeNodes.Application.Common.DTOs;
using TreeNodes.Auth.Interfaces;

namespace TreeNodes.Web.Controllers;

[ApiController]
[Route("")]
[AllowAnonymous] // Auth controller should not require authentication
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates user by unique partner code and returns JWT auth token.
    /// Throws InvalidPartnerCodeException if authentication fails.
    /// </summary>
    /// <param name="code">Partner authentication code</param>
    /// <returns>Token information containing JWT bearer token</returns>
    /// <exception cref="TreeNodes.Auth.Exceptions.InvalidPartnerCodeException">Thrown when partner code is invalid or missing</exception>
    [HttpPost("api.user.partner.rememberMe")]
    public ActionResult<TokenInfoDto> RememberMe([FromQuery] string code)
    {
        // Let the service throw exceptions, which will be caught by GlobalExceptionHandlingMiddleware
        // This ensures authentication failures are logged to the journal
        var token = _authService.Authenticate(code);

        return Ok(new TokenInfoDto { Token = token });
    }
}



