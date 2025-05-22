namespace EvrenDev.Application.Features.Donations.Fountain.Models;

public record WeeklyReportDto
{
    public FountainDonationProject? Project { get; set; }
    public FountainItemDto? LastOnlineFountain { get; set; }
    public List<FountainItemDto>? PendingMediaFountains { get; set; }
    public FountainItemDto? LastAssignedFountainCode { get; set; }
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
