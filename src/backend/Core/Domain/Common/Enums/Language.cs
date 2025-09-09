namespace EvrenDev.Domain.Common.Enums;

public enum Language
{
    None = 0,
    En = 1,
    Tr = 2,
    De = 3,
    Fr = 4,
    Es = 5,
    It = 6
}

public static class LanguageExtensions
{
    public static string GetValue(this Language language)
    {
        return language switch
        {
            Language.None => "none",
            Language.En => "en",
            Language.Tr => "tr",
            Language.De => "de",
            Language.Fr => "fr",
            Language.Es => "es",
            Language.It => "it",
            _ => throw new ArgumentOutOfRangeException(nameof(language))
        };
    }

    public static Language FromInt(int value)
    {
        return value switch
        {
            0 => Language.None,
            1 => Language.En,
            2 => Language.Tr,
            3 => Language.De,
            4 => Language.Fr,
            5 => Language.Es,
            6 => Language.It,
            _ => Language.None
        };
    }

    public static List<LanguageInfo> ToList()
    {
        return [.. Enum.GetValues<Language>()
            .Select(l => new LanguageInfo
            {
                Name = GetValue(l),
                Value = l
            })
            .Cast<LanguageInfo>()];
    }

    public class LanguageInfo
    {
        public string? Name { get; set; }
        public Language Value { get; set; }
    }
}
