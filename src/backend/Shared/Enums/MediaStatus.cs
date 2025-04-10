namespace EvrenDev.Shared.Enums;

public class MediaStatus(string? name, string? backgroundColor)
{
    public string? Name { get; private set; } = name;
    public string? BackgroundColor { get; private set; } = backgroundColor;

    // Predefined statuses
    public static MediaStatus None => new("none", "none");
    public static MediaStatus Missing => new("missing", "error");
    public static MediaStatus Arrived => new("arrived", "lighterror");
    public static MediaStatus Online => new("online", "success");
    public static MediaStatus Edited => new("edited", "warning");
    public static MediaStatus Transferred => new("transferred", "info");
    public static MediaStatus Reviewed => new("reviewed", "primary");


    public static MediaStatus From(string? name)
    {
        return ToList
            .FirstOrDefault(status => string.Equals(status.Name, name, StringComparison.OrdinalIgnoreCase)) ?? None;
    }

    public static IEnumerable<MediaStatus> ToList
    {
        get
        {
            yield return Missing;
            yield return Arrived;
            yield return Edited;
            yield return Online;
            yield return Transferred;
            yield return Reviewed;
        }
    }
}
