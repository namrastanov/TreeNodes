namespace TreeNodes.Auth.Exceptions;

/// <summary>
/// Exception thrown when a JWT token is invalid or expired.
/// This exception should be caught by the global exception handler for logging.
/// </summary>
public class InvalidTokenException : AuthenticationException
{
    public string? Token { get; }

    public InvalidTokenException(string message) : base(message)
    {
    }

    public InvalidTokenException(string message, string token) : base(message)
    {
        Token = token;
    }

    public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

