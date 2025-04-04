namespace EvrenDev.Shared.Constants;

public abstract class Policies
{
    public static class Permissions
    {
        public const string Restore = nameof(Restore);
        public const string Read = nameof(Read);
        public const string Edit = nameof(Edit);
        public const string Delete = nameof(Delete);
        public const string Create = nameof(Create);
    }

    public static class Modules
    {
        public const string Donations = nameof(Donations);
        public const string Todos = nameof(Todos);
        public const string Tenants = nameof(Tenants);
        public const string Roles = nameof(Roles);
        public const string Users = nameof(Users);
        public const string Audits = nameof(Audits);
        public const string Files = nameof(Files);
        public const string Images = nameof(Images);
    }

    public static string[] AllModules => [
        Modules.Donations,
        Modules.Todos,
        Modules.Tenants,
        Modules.Roles,
        Modules.Images,
        Modules.Users,
        Modules.Audits,
        Modules.Files,
    ];

    public static string[] AllPermissions => [
        Permissions.Restore,
        Permissions.Read,
        Permissions.Edit,
        Permissions.Delete,
        Permissions.Create
    ];

    public static string[] DefaultModules => [
        Modules.Donations,
        Modules.Todos,
        Modules.Audits,
        Modules.Files,
    ];

    public static string[] DefaultPermissions => [
        Permissions.Read,
        Permissions.Edit,
        Permissions.Delete,
        Permissions.Create
    ];

    public static string[] AllModulesWithPermissions => AllModules.SelectMany(module => AllPermissions.Select(permission => $"{module}.{permission}")).ToArray();

    public static string[] DefaultModulesWithPermissions => DefaultModules.SelectMany(module => DefaultPermissions.Select(permission => $"{module}.{permission}")).ToArray();
}
