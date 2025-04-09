namespace EvrenDev.Shared.Enums;

public class FontainDonationProject(string? name, string? alias)
{
    public string? Name { get; private set; } = name;
    public string? Alias { get; private set; } = alias;

    // Predefined statuses
    public static FontainDonationProject Bks => new("BKS", "BL01");
    public static FontainDonationProject Bgs => new("BGS", "BL01");
    public static FontainDonationProject Aki => new("AKI", "AF01");
    public static FontainDonationProject Agi => new("AGI", "AF02");

    public static FontainDonationProject From(string? name)
    {
        return ToList
            .FirstOrDefault(status => string.Equals(status.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Bks;
    }

    public static IEnumerable<FontainDonationProject> ToList
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
