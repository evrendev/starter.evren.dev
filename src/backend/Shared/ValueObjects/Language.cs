namespace EvrenDev.Shared.ValueObjects;

public class Language(string code) : ValueObject
{
    public static Language From(string code)
    {
        var language = new Language(code);

        if (!SupportedLanguages.Contains(language))
        {
            throw new UnsupportedLanguageException(code);
        }

        return language;
    }

    public static Language Deutsch => new("de");

    public static Language English => new("en");

    public static Language Turkce => new("tr");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? Defaults.Language : code;

    public static implicit operator string(Language language)
    {
        return language.ToString();
    }

    public static explicit operator Language(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<Language> SupportedLanguages
    {
        get
        {
            yield return Deutsch;
            yield return English;
            yield return Turkce;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
