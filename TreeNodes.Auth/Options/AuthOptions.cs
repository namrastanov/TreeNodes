namespace TreeNodes.Auth.Options;

/// <summary>
/// Configuration options for authentication
/// </summary>
public class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// The authorized partner code
    /// </summary>
    public string AuthorizedCode { get; set; } = string.Empty;

    /// <summary>
    /// JWT secret key for token signing
    /// </summary>
    public string JwtSecret { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string JwtIssuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience
    /// </summary>
    public string JwtAudience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes
    /// </summary>
    public int TokenExpirationMinutes { get; set; } = 60;
}

