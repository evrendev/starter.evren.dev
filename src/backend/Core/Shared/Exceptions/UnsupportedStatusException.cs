namespace EvrenDev.Shared.Exceptions;

public class UnsupportedStatusException : Exception
{
    public UnsupportedStatusException(string? code)
        : base($"Status \"{code}\" is unsupported.")
    {
    }
}
