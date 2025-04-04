namespace EvrenDev.Application.Features.Donations.BrunnenDonations.Models;
public record FullBrunnenDonationDto
{
    public Guid Id { get; set; }
    public string? Contact { get; set; }
    public string? Phone { get; set; }
    public string? Project { get; set; }
    public string? Banner { get; set; }
    public DateTimeDto? CreationDate { get; set; }
    public int? ProjectNumber { get; set; }
    public string? ProjectCode { get; set; }
    public string? TransactionId { get; set; }
    public string? Source { get; set; }
    public string? Info { get; set; }
}
