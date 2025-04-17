namespace EvrenDev.Shared.Enums;

public class FountainDonationProject(string? name, string? alias)
{
    public string? Name { get; private set; } = name;
    public string? Alias { get; private set; } = alias;

    // Predefined statuses
    public static FountainDonationProject Bks => new("BKS", "BL01");
    public static FountainDonationProject Bgs => new("BGS", "BL02");
    public static FountainDonationProject Aki => new("AKI", "AF01");
    public static FountainDonationProject Agi => new("AGI", "AF02");

    public static FountainDonationProject FromName(string? name)
    {
        return ToList
            .FirstOrDefault(project => string.Equals(project.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Bks;
    }

    public static FountainDonationProject FromAlias(string? alias)
    {
        return ToList
            .FirstOrDefault(project => string.Equals(project.Name, alias, StringComparison.OrdinalIgnoreCase)) ?? Bks;
    }

    public static IEnumerable<FountainDonationProject> ToList
    {
        get
        {
            yield return Bks;
            yield return Bgs;
            yield return Aki;
            yield return Agi;
        }
    }
}
