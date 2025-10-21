namespace TreeNodes.Auth.Interfaces;

/// <summary>
/// Service for partner authentication
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a partner by their code
    /// </summary>
    /// <param name="code">The partner code to authenticate</param>
    /// <returns>JWT token if authentication is successful</returns>
    /// <exception cref="Auth.Exceptions.InvalidPartnerCodeException">Thrown when partner code is invalid or empty</exception>
    string? Authenticate(string code);
}

