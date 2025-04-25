namespace EvrenDev.Application.Features.Donations.Fountain.Models;
public record ProjectsCountDto
{
    public FountainDonationProject? Project { get; set; }
    public int Count { get; set; }
}
