using EvrenDev.Domain.Common;

namespace EvrenDev.Domain.Entities.Tenant;

public class Tenant : BaseEntity
{
    public string? Name { get; set; }
    public string? ConnectionString { get; set; }
    public string? Host { get; set; }
    public bool IsActive { get; set; }
}
