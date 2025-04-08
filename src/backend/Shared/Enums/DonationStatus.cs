namespace EvrenDev.Shared.Enums;

public class DonationStatus(string? name, string? backgroundColor)
{
    public string? Name { get; private set; } = name;
    public string? BackgroundColor { get; private set; } = backgroundColor;

    // Predefined statuses
    public static DonationStatus NoDetails => new("noDetails", "lightsecondary");
    public static DonationStatus InitialWeek => new("initialWeek", "primary");
    public static DonationStatus OngoingEarlyWeeks => new("ongoingEarlyWeeks", "info");
    public static DonationStatus Week5Media => new("week5Media", "warning");
    public static DonationStatus Week6Warning => new("week6Warning", "lighterror");
    public static DonationStatus Week8Critical => new("week8Critical", "error");
    public static DonationStatus Published => new("published", "success");

    public static DonationStatus From(string? name)
    {
        return SupportedDonationStatuses
            .FirstOrDefault(status => string.Equals(status.Name, name, StringComparison.OrdinalIgnoreCase)) ?? NoDetails;
    }

    public static IEnumerable<DonationStatus> SupportedDonationStatuses
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
