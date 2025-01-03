//NOTE: Argue salutation codes with the team
// NOTE: This is a value object class that represents the salutation of the donor.

namespace EvrenDev.Shared.ValueObjects;

public class Salutation(string code) : ValueObject
{
    public static Salutation From(string code)
    {
        var salutation = new Salutation(code);

        if (!SupportedSalutations.Contains(salutation))
        {
            throw new UnsupportedSalutationException(code);
        }

        return salutation;
    }

    public static Salutation None => new("none");

    public static Salutation Mr => new("mr");

    public static Salutation Mrs => new("mrs");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? Defaults.Salutation : code;

    public static implicit operator string(Salutation salutation)
    {
        return salutation.ToString();
    }

    public static explicit operator Salutation(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<Salutation> SupportedSalutations
    {
        get
        {
            yield return None;
            yield return Mr;
            yield return Mrs;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
