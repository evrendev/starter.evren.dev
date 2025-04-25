namespace EvrenDev.Application.Features.Donations.Fountain.Models;
public record ProjectsCountDto
{
    public string? ProjectCode { get; set; }
    public int Count { get; set; }
}
