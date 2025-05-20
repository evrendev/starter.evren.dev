namespace EvrenDev.Application.Features.Absences.Models;

public class AbsenceDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Start { get; set; }
    public string? End { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? Calendar { get; set; }
}
