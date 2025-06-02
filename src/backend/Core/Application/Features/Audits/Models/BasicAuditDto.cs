namespace EvrenDev.Application.Features.Audits.Models;

public class BasicAuditDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public DateTimeDto? DateTime { get; set; }
    public AuditAction? Action { get; set; }
    public string? EntityType { get; set; }
}
