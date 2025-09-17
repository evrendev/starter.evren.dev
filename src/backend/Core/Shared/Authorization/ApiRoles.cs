using System.Collections.ObjectModel;

namespace EvrenDev.Shared.Authorization;

public static class ApiRoles
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[] { Admin, Basic });

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(r => r == roleName);
    }
}
