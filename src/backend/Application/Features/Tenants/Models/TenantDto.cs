namespace EvrenDev.Application.Features.Tenants.Models;

public class TenantDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; }
}
