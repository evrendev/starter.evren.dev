namespace EvrenDev.Application.Features.Donations.Fountain.Models;

public record DonationMetrics
{
    public IReadOnlyList<ProjectsCountDto>? TotalFountainCountsByProject { get; set; }
    public IReadOnlyList<BasicFountainDonationDto>? RecentFountainDonations { get; set; }
    public List<MonthlyProjectStatsDto>? MonthlyProjectStats { get; set; }
}
