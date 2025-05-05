namespace EvrenDev.Shared.Enums;

public class FountainDonationProject(string? name, string? color)
{
    public string? Name { get; private set; } = name;
    public string? Color { get; private set; } = color;

    public static FountainDonationProject BL01 => new("BL01", "primary");
    public static FountainDonationProject BL02 => new("BL02", "info");
    public static FountainDonationProject AF01 => new("AF01", "warning");
    public static FountainDonationProject AF02 => new("AF02", "error");

    public static FountainDonationProject FromName(string? name)
    {
        return ToList
            .FirstOrDefault(project => string.Equals(project.Name, name, StringComparison.OrdinalIgnoreCase)) ?? BL01;
    }

    public static IEnumerable<FountainDonationProject> ToList
    {
        get
        {
            yield return BL01;
            yield return BL02;
            yield return AF01;
            yield return AF02;
        }
    }
}
