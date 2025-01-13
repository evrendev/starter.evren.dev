namespace EvrenDev.Domain.Entities.Tenant;

public class TenantEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
}
