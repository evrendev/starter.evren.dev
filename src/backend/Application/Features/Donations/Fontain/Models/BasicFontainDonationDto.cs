namespace EvrenDev.Application.Features.Donations.FontainDonations.Models;
public record BasicFontainDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public DateTimeDto? CreationDate { get; set; }
    public int Weeks =>
        CreationDate?.UtcDateTime is DateTime creation
            ? (int)Math.Round((DateTime.Today - creation).TotalDays / 7.0)
            : 0;

    public DonationStatus Status =>
    Weeks switch
    {
        <= 1 => DonationStatus.InitialWeek,
        <= 4 => DonationStatus.OngoingEarlyWeeks,
        5 => DonationStatus.Week5Media,
        6 => DonationStatus.Week6Warning,
        >= 8 => DonationStatus.Week8Critical,
        _ => DonationStatus.Published
    };

    public string? HtmlBanner { get; set; }
    public string? PlainBanner { get; set; }
    public string? Team { get; set; }
}
