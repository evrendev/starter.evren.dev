namespace EvrenDev.Shared.Constants;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
    public const string Management = nameof(Management);
    public const string Finance = nameof(Finance);
    public const string Editor = nameof(Editor);
    public const string User = nameof(User);
    public static string[] AllRoles => [
        Administrator,
        Management,
        Finance,
        Editor,
        User
    ];
}
