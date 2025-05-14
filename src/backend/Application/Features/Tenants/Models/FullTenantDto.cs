namespace EvrenDev.Application.Features.Tenants.Models;

public class FullTenantDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTimeDto? ValidUntil { get; set; }
    public string? Description { get; set; }
    public bool Deleted { get; set; }
}
