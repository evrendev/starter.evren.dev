namespace EvrenDev.Domain.Common.Enums;

public enum Language
{
    En = 1,
    Tr = 2,
    De = 3,
}

public static class LanguageExtensions
{
    public static string GetValue(this Language language)
    {
        return language switch
        {
            Language.En => "en",
            Language.Tr => "tr",
            Language.De => "de",
            _ => "en",
        };
    }

    public static Language FromInt(int value)
    {
        return value switch
        {
            1 => Language.En,
            2 => Language.Tr,
            3 => Language.De,
            _ => Language.En,
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
