namespace EvrenDev.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
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
    }

    public void Delete(string? deleter)
    {
        Deleted = true;
        Deleter = deleter;
        DeletionTime = DateTimeOffset.Now;
    }
}
