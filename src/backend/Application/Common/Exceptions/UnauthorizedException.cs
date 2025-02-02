namespace EvrenDev.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a user attempts to access a resource they don't have permission for
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ForbiddenException class
    /// </summary>
    public UnauthorizedException() : base("User is not authenticated")
    {
    }


    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with a custom message
    /// </summary>
    /// <param name="message">The custom error message</param>
    public UnauthorizedException(string message, string v) : base(message)
    {
    }


    /// <summary>
    /// Initializes a new instance of the ForbiddenException class with a custom message and inner exception
    /// </summary>
    /// <param name="message">The custom error message</param>
    /// <param name="innerException">The inner exception</param>
    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
