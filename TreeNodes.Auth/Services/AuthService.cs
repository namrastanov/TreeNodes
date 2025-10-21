using Microsoft.Extensions.Options;
using TreeNodes.Auth.Exceptions;
using TreeNodes.Auth.Interfaces;
using TreeNodes.Auth.Options;

namespace TreeNodes.Auth.Services;

/// <summary>
/// Implementation of authentication service
/// </summary>
public class AuthService : IAuthService
{
    private readonly AuthOptions _authOptions;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IOptions<AuthOptions> authOptions,
        IJwtTokenService jwtTokenService)
    {
        _authOptions = authOptions.Value;
        _jwtTokenService = jwtTokenService;
    }

    public string? Authenticate(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidPartnerCodeException("Partner code is required");
        }

        // Validate the provided code against the configured authorized code
        if (!code.Equals(_authOptions.AuthorizedCode, StringComparison.Ordinal))
        {
            throw new InvalidPartnerCodeException("Invalid partner code", code);
        }

        // Generate and return JWT token
        return _jwtTokenService.GenerateToken(code);
    }
}

