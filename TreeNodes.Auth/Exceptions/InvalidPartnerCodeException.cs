namespace TreeNodes.Auth.Exceptions;

/// <summary>
/// Exception thrown when an invalid partner code is provided.
/// This exception should be caught by the global exception handler for logging.
/// </summary>
public class InvalidPartnerCodeException : AuthenticationException
{
    public InvalidPartnerCodeException(string message) : base(message)
    {
    }

    public InvalidPartnerCodeException(string message, string code) : base(message, code)
    {
    }
}

