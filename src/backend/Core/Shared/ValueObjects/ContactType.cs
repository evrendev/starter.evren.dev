namespace EvrenDev.Shared.ValueObjects;

public class ContactType(string code) : ValueObject
{
    public static ContactType From(string code)
    {
        var contacttype = new ContactType(code);

        if (!SupportedContactTypes.Contains(contacttype))
        {
            throw new UnsupportedContactTypeException(code);
        }

        return contacttype;
    }

    public static ContactType Home => new("home");

    public static ContactType Work => new("work");

    public static ContactType Other => new("other");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? Defaults.ContactType : code;

    public static implicit operator string(ContactType contacttype)
    {
        return contacttype.ToString();
    }

    public static explicit operator ContactType(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    public static IEnumerable<ContactType> SupportedContactTypes
    {
        get
        {
            yield return Home;
            yield return Work;
            yield return Other;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
