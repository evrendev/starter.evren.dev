namespace EvrenDev.Shared.Enums;

public class FountaionTeam(string? name, string? backgroundColor)
{
    public string? Name { get; private set; } = name;
    public string? BackgroundColor { get; private set; } = backgroundColor;

    // Predefined statuses
    public static FountaionTeam None => new("none", "secondary");
    public static FountaionTeam Morteza => new("morteza", "success");
    public static FountaionTeam Idris => new("idris", "info");

    public static FountaionTeam From(string? name)
    {
        var response = ToList.FirstOrDefault(status => status.Name == name) ?? None;

        return response;
    }

    public static IEnumerable<FountaionTeam> ToList
    {
        get
        {
            yield return None;
            yield return Morteza;
            yield return Idris;
        }
    }
}
