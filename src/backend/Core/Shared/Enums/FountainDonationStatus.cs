namespace EvrenDev.Shared.Enums;

public class FountainDonationStatus(string? name, string? backgroundColor)
{
    public string? Name { get; private set; } = name;
    public string? BackgroundColor { get; private set; } = backgroundColor;

    // Predefined statuses
    public static FountainDonationStatus NoDetails => new("noDetails", "lightsecondary");
    public static FountainDonationStatus InitialWeek => new("initialWeek", "primary");
    public static FountainDonationStatus OngoingEarlyWeeks => new("ongoingEarlyWeeks", "info");
    public static FountainDonationStatus Week5Media => new("week5Media", "warning");
    public static FountainDonationStatus Week6Warning => new("week6Warning", "lighterror");
    public static FountainDonationStatus Week8Critical => new("week8Critical", "error");
    public static FountainDonationStatus Published => new("published", "success");

    public static FountainDonationStatus From(string? name)
    {
        return ToList
            .FirstOrDefault(status => string.Equals(status.Name, name, StringComparison.OrdinalIgnoreCase)) ?? NoDetails;
    }

    public static IEnumerable<FountainDonationStatus> ToList
    {
        get
        {
            yield return NoDetails;
            yield return InitialWeek;
            yield return OngoingEarlyWeeks;
            yield return Week5Media;
            yield return Week6Warning;
            yield return Week8Critical;
            yield return Published;
        }
    }
}
