namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user attempts to access a resource they don't have permission for
/// </summary>
public class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ForbiddenAccessException class
    /// </summary>
    public ForbiddenAccessException()
        : base("You do not have permission to access this resource.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the ForbiddenAccessException class with a custom message
    /// </summary>
    /// <param name="message">The custom error message</param>
    public ForbiddenAccessException(string message, string v)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the ForbiddenAccessException class with a custom message and inner exception
    /// </summary>
    /// <param name="message">The custom error message</param>
    /// <param name="innerException">The inner exception</param>
    public ForbiddenAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
