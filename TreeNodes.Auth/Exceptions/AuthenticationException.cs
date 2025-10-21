namespace TreeNodes.Auth.Exceptions;

/// <summary>
/// Exception thrown when authentication fails.
/// This exception should be caught by the global exception handler for logging.
/// </summary>
public class AuthenticationException : Exception
{
    public string? Code { get; }

    public AuthenticationException(string message) : base(message)
    {
    }

    public AuthenticationException(string message, string code) : base(message)
    {
        Code = code;
    }

    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

