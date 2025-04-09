namespace EvrenDev.Application.Features.Donations.Fontain.Models;
public record FullFontainDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public string? Project { get; set; }
    public string? Banner { get; set; }
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
    public string? ProjectCode { get; set; }
    public string? TransactionId { get; set; }
    public string? Source { get; set; }
    public string? Info { get; set; }
    public string? Team { get; set; }
}
