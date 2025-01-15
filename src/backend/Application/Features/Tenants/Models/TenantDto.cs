namespace EvrenDev.Application.Features.Tenants.Models;

public class TenantDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Description { get; set; }
    public bool Deleted { get; set; }
}
