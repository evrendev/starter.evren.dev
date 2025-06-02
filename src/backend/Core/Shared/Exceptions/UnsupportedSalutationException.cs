namespace EvrenDev.Shared.Exceptions;

public class UnsupportedSalutationException : Exception
{
    public UnsupportedSalutationException(string? code)
        : base($"Salutation \"{code}\" is unsupported.")
    {
    }
}
