using System.Globalization;

namespace EvrenDev.Application.Features.Donations.Fountain.Models;

public record WeeklyReportDto
{
    public int? IsoWeekNumber => GetIso8601WeekOfYear(DateTime.Today);
    public int? IsoYear => DateTime.Today.Year;
    public List<ProjectReportDto>? Projects { get; set; }
    private static int GetIso8601WeekOfYear(DateTime time)
    {
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            time = time.AddDays(3);

        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
            time,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday
        );
    }
}

public record ProjectReportDto
{
    public FountainDonationProject? Project { get; set; }
    public FountainItemDto? LastOnlineFountain { get; set; }
    public List<FountainItemDto>? PendingMediaFountains { get; set; }
    public FountainItemDto? LastAssignedFountain { get; set; }
    public List<FountainItemDto>? MissingSince6Weeks { get; set; }
    public List<FountainItemDto>? MissingSince8Weeks { get; set; }
    public List<FountainItemDto>? MissingSince13Weeks { get; set; }
}

public record FountainItemDto
{
    public string? Code { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? Date => CreationDate?.ToString("dd MMM yyyy");
    public int Weeks => CreationDate.HasValue
        ? (int)Math.Round((DateTime.Today - CreationDate.Value).TotalDays / 7.0)
        : 0;
}
