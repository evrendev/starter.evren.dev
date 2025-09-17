namespace EvrenDev.Application.Catalog.Absences.Entities;

public class AbsenceDto : IDto
{
    public Guid Id { get; set; }
    public string? Title => $"{Employee} - {Location}{(string.IsNullOrEmpty(Description) ? "" : $" - {Description}")}";
    public DateTime StartDate { get; set; } = default!;
    public DateTime EndDate { get; set; } = default!;
    public string? Description { get; set; }
    public string Location { get; set; } = default!;
    public string Employee { get; set; } = default!;
    public string CalendarId { get; set; } = default!;
}
