namespace EvrenDev.Domain.Entities.Tenant;

public class TenantEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
}
