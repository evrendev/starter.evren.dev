namespace EvrenDev.Shared.Constants;

public abstract class Policies
{
    public class Permissions
    {
        public const string ReadOnly = nameof(ReadOnly);
        public const string FullAccess = nameof(FullAccess);
        public const string NoAccess = nameof(NoAccess);
    }

    public class Modules
    {
        public const string Todos = nameof(Todos);
        public const string Tenants = nameof(Tenants);
        public const string Permissions = nameof(Permissions);
        public const string Roles = nameof(Roles);
        public const string Users = nameof(Users);
        public const string Logs = nameof(Logs);
    }

    public static string[] AllModules => [
        Modules.Todos,
        Modules.Tenants,
        Modules.Permissions,
        Modules.Roles,
        Modules.Users,
        Modules.Logs,
    ];

    public static string[] AllPermissions => [
        Permissions.NoAccess,
        Permissions.ReadOnly,
        Permissions.FullAccess
    ];

    public static string[] AllModulesWithPermissions => AllModules.SelectMany(module => AllPermissions.Select(permission => $"{module}.{permission}")).ToArray();
}
