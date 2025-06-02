namespace EvrenDev.Domain.Entities.Audit;

[AuditIgnore]
public class AuditLog
{
    public Guid Id { get; set; }
    public string? IpAddress { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? AuditData { get; set; }
    public string? EntityType { get; set; }
    public DateTime? AuditDateTimeUtc { get; set; }
    public string? TablePk { get; set; }
    public string? Action { get; set; }
}
