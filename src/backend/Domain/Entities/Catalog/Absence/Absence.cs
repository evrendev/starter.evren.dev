namespace EvrenDev.Domain.Entities.Catalog;

[AuditInclude]
public class Absence : BaseAuditableEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? CalendarId { get; set; }
}
