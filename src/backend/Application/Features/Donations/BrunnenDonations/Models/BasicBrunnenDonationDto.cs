namespace EvrenDev.Application.Features.Donations.BrunnenDonations.Models;
public record BasicBrunnenDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public DateTimeDto? CreationDate { get; set; }
    public int Weeks =>
        CreationDate?.UtcDateTime is DateTime creation
            ? (int)Math.Round((DateTime.Today - creation).TotalDays / 7.0)
            : 0;
    public string? Info { get; set; }
    public string? Team { get; set; }
}
