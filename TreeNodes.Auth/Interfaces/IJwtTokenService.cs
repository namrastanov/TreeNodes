namespace TreeNodes.Auth.Interfaces;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the given partner code
    /// </summary>
    /// <param name="partnerCode">The partner code to encode in the token</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(string partnerCode);

    /// <summary>
    /// Validates a JWT token and returns the partner code if valid
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>Partner code if token is valid, null otherwise</returns>
    string? ValidateToken(string token);
}

