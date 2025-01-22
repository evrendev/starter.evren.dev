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
        public const string Todos = nameof(Todos);
        public const string Tenants = nameof(Tenants);
        public const string Permissions = nameof(Permissions);
        public const string Roles = nameof(Roles);
        public const string Images = nameof(Images);
        public const string Users = nameof(Users);
        public const string Logs = nameof(Logs);
        public const string Products = "Products";
        public const string Brands = "Brands";
        public const string Categories = "Categories";
        public const string Files = "Files";
    }

    public static string[] AllModules => [
        Modules.Todos,
        Modules.Tenants,
        Modules.Permissions,
        Modules.Roles,
        Modules.Images,
        Modules.Users,
        Modules.Logs,
        Modules.Products,
        Modules.Brands,
        Modules.Categories,
        Modules.Files,
    ];

    public static string[] AllPermissions => [
        Permissions.Restore,
        Permissions.Read,
        Permissions.Edit,
        Permissions.Delete,
        Permissions.Create
    ];

    public static string[] AllModulesWithPermissions => AllModules.SelectMany(module => AllPermissions.Select(permission => $"{module}.{permission}")).ToArray();
}
