namespace EvrenDev.Shared.ValueObjects;

public class Gender(string code) : ValueObject
{
    public static Gender From(string code)
    {
        var gender = new Gender(code);

        if (!SupportedGenders.Contains(gender))
        {
            throw new UnsupportedGenderException(code);
        }

        return gender;
    }

    public static Gender None => new("none");

    public static Gender Mrs => new("mrs");

    public static Gender Mr => new("mr");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? Defaults.Gender : code;

    public static implicit operator string(Gender gender)
    {
        return gender.ToString();
    }

    public static explicit operator Gender(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<Gender> SupportedGenders
    {
        get
        {
            yield return None;
            yield return Mrs;
            yield return Mr;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
