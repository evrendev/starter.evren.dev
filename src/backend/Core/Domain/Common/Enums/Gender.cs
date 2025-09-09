namespace EvrenDev.Domain.Common.Enums;

public enum Gender
{
    None = 0,
    Mr = 1,
    Mrs = 2,
    Miss = 3,
    Other = 4
}

public static class GenderExtensions
{
    public static string GetValue(this Gender gender)
    {
        return gender switch
        {
            Gender.None => "none",
            Gender.Mr => "mr",
            Gender.Mrs => "mrs",
            Gender.Miss => "miss",
            Gender.Other => "other",
            _ => throw new ArgumentOutOfRangeException(nameof(gender))
        };
    }

    public static Gender FromInt(int value)
    {
        return value switch
        {
            0 => Gender.None,
            1 => Gender.Mr,
            2 => Gender.Mrs,
            3 => Gender.Miss,
            4 => Gender.Other,
            _ => Gender.None
        };
    }

    public static List<GenderInfo> ToList()
    {
        return [.. Enum.GetValues<Gender>()
            .Select(g => new GenderInfo
            {
                Name = GetValue(g),
                Value = g
            })
            .Cast<GenderInfo>()];
    }

    public class GenderInfo
    {
        public string? Name { get; set; }
        public Gender Value { get; set; }
    }
}
