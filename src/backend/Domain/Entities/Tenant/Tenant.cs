using Finbuckle.MultiTenant.Abstractions;

namespace EvrenDev.Domain.Entities.Tenant;

[AuditInclude]
public class TenantEntity : ITenantInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Identifier { get; set; }
    public bool IsActive { get; set; }
    public string? AdminEmail { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
    public string? Creator { get; set; }
    public DateTimeOffset ModifiedTime { get; set; }
    public string? Modifier { get; set; }
    public bool Deleted { get; set; }
    public string? Deleter { get; set; }
    public DateTimeOffset? DeletionTime { get; set; }
    public void Restore()
    {
        Deleted = false;
        Deleter = null;
        DeletionTime = null;
        IsActive = true;
    }
    public void Delete(string? deleter)
    {
        Deleted = true;
        Deleter = deleter;
        DeletionTime = DateTimeOffset.Now;
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
        ValidUntil = DateTime.Now.AddYears(1);
        Deleted = false;
        Deleter = null;
        DeletionTime = null;
    }

    public void Deactivate()
    {
        IsActive = false;
        ValidUntil = DateTime.Now.AddYears(-1);
        Deleted = false;
        Deleter = null;
        DeletionTime = null;
    }
}
