namespace EvrenDev.Shared.Constants;

public abstract class Roles
{
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string Administrator = nameof(Administrator);
    public const string Management = nameof(Management);
    public const string Finance = nameof(Finance);
    public const string Editor = nameof(Editor);
    public const string User = nameof(User);
    public static string[] ToList => [
        SuperAdmin,
        Administrator,
        Management,
        Finance,
        Editor,
        User
    ];
}
