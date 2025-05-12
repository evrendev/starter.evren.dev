using EvrenDev.Domain.Interfaces;

namespace EvrenDev.Domain.Entities.Catalog;

[AuditInclude]
public class Absence : BaseAuditableEntity, ITenant
{
    public string? Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Employee { get; set; }
    public string? Calendar { get; set; }
}
