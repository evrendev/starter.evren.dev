namespace EvrenDev.Application.Features.Donations.BrunnenDonations.Models;
public record BasicBrunnenDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public DateTimeDto? CreationDate { get; set; }
    public string? Info { get; set; }
}
