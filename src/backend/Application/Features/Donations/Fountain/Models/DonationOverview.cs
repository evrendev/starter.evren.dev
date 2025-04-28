namespace EvrenDev.Application.Features.Donations.Fountain.Models;
public record DonationOverview
{
    public IReadOnlyList<ProjectsCountDto>? Stats { get; set; }

    public IReadOnlyList<BasicFountainDonationDto>? Donations { get; set; }
}
