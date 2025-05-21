namespace EvrenDev.Application.Features.Donations.Fountain.Models;

public class MonthlyProjectStatsDto
{
    public string? Month { get; set; }
    public FountainDonationProject? Project { get; set; }
    public int Count { get; set; }
}
