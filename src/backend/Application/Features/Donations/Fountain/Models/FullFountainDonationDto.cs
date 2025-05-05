using static EvrenDev.Application.Common.Functions.Tools;

namespace EvrenDev.Application.Features.Donations.Fountain.Models;
public record FullFountainDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public Phone? Phone { get; set; }
    public string? Project { get; set; }
    public string? HtmlBanner { get; set; }
    public string? PlainBanner { get; set; }
    public DateTimeDto? CreationDate { get; set; }
    public int Weeks =>
        CreationDate?.UtcDateTime is DateTime creation
            ? (int)Math.Round((DateTime.Today - creation).TotalDays / 7.0)
            : 0;

    public FountainDonationStatus Status =>
    Weeks switch
    {
        <= 1 => FountainDonationStatus.InitialWeek,
        <= 4 => FountainDonationStatus.OngoingEarlyWeeks,
        5 => FountainDonationStatus.Week5Media,
        6 => FountainDonationStatus.Week6Warning,
        >= 8 => FountainDonationStatus.Week8Critical,
        _ => FountainDonationStatus.Published
    };
    public int? ProjectNumber { get; set; }
    public string? TransactionId { get; set; }
    public string? Source { get; set; }
    public string? Info { get; set; }
    public FountaionTeam? Team { get; set; }
    public MediaStatus? MediaStatus { get; set; }
    public string? MediaInformation { get; set; }
    public bool IsDonorNotified { get; set; }
    public bool IsConstructionTeamNotified { get; set; }
}
