namespace EvrenDev.Shared.Exceptions;

public class UnsupportedLanguageException : Exception
{
    public UnsupportedLanguageException(string? code)
        : base($"Language \"{code}\" is unsupported.")
    {
    }
}
