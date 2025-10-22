namespace TreeNodes.Application.Common.Exceptions;

/// <summary>
/// Base exception for secure errors that should return a safe message.
/// </summary>
public class SecureException : Exception
{
    public SecureException(string message) : base(message) { }
    public SecureException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when attempting to delete a node that has children.
/// </summary>
public class DeleteChildrenFirstException : SecureException
{
    public DeleteChildrenFirstException() : base("You have to delete all children nodes first") { }
}

/// <summary>
/// Exception thrown when a node name duplicates among siblings.
/// </summary>
public class NodeNameNotUniqueException : SecureException
{
    public NodeNameNotUniqueException(string name) : base($"Node name '{name}' must be unique among siblings") { }
}

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}


