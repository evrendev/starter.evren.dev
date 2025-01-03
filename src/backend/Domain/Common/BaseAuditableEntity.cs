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
}
