namespace EvrenDev.Shared.Enums;

public class AuditAction(string? name, string? backgroundColor)
{

    public string? Name { get; private set; } = name;

    public string? BackgroundColor { get; private set; } = backgroundColor;

    public static AuditAction None => new(name: null,
        backgroundColor: "bg-secondary-gradient"
    );

    public static AuditAction Insert => new(name: "Insert",
        backgroundColor: "bg-success-gradient"
    );

    public static AuditAction Update => new(name: "Update",
        backgroundColor: "bg-warning-gradient"
    );

    public static AuditAction Delete => new(name: "Delete",
        backgroundColor: "bg-danger-gradient"
    );

    public static AuditAction Recovered => new(name: "Recovered",
        backgroundColor: "bg-primary-gradient"
    );

    public static AuditAction From(string? name)
    {
        return SupportedAuditActions.Single(audit => string.Equals(audit.Name, name, StringComparison.OrdinalIgnoreCase)) ?? None;
    }

    public static IEnumerable<AuditAction> SupportedAuditActions
    {
        get
        {
            yield return Insert;
            yield return Update;
            yield return Delete;
            yield return Recovered;
        }
    }
}
