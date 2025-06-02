namespace EvrenDev.Shared.ValueObjects;

public class Status(string code) : ValueObject
{
    public static Status From(string code)
    {
        var status = new Status(code);

        if (!SupportedStatuss.Contains(status))
        {
            throw new UnsupportedStatusException(code);
        }

        return status;
    }

    public static Status Waiting => new("waiting");

    public static Status Confirmed => new("confirmed");

    public static Status Reversed => new("reversed");

    public static Status Failed => new("failed");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? Defaults.Status : code;

    public static implicit operator string(Status status)
    {
        return status.ToString();
    }

    public static explicit operator Status(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<Status> SupportedStatuss
    {
        get
        {
            yield return Waiting;
            yield return Confirmed;
            yield return Reversed;
            yield return Failed;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
