namespace EvrenDev.Application.Features.Tenants.Models;

public class BasicTenantDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTimeDto? ValidUntil { get; set; }
    public bool Deleted { get; set; }
}
