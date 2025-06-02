namespace EvrenDev.Shared.Exceptions;

public class UnsupportedContactTypeException : Exception
{
    public UnsupportedContactTypeException(string? code)
        : base($"ContactType \"{code}\" is unsupported.")
    {
    }
}
