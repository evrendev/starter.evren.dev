namespace EvrenDev.Shared.Exceptions;

public class UnsupportedGenderException : Exception
{
    public UnsupportedGenderException(string? code)
        : base($"Gender \"{code}\" is unsupported.")
    {
    }
}
