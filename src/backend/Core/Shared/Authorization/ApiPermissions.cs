using System.Collections.ObjectModel;

namespace EvrenDev.Shared.Authorization;

public static class ApiPermissions
{
    private static readonly ApiPermission[] AllClaims =
    {
        new("View Dashboard", ApiAction.View, ApiResource.Dashboard),
        new("View Hangfire", ApiAction.View, ApiResource.Hangfire),
        new("View Users", ApiAction.View, ApiResource.Users),
        new("Search Users", ApiAction.Search, ApiResource.Users),
        new("Create Users", ApiAction.Create, ApiResource.Users),
        new("Update Users", ApiAction.Update, ApiResource.Users),
        new("Delete Users", ApiAction.Delete, ApiResource.Users),
        new("Export Users", ApiAction.Export, ApiResource.Users),
        new("View UserRoles", ApiAction.View, ApiResource.UserRoles),
        new("Update UserRoles", ApiAction.Update, ApiResource.UserRoles),
        new("View Roles", ApiAction.View, ApiResource.Roles),
        new("Create Roles", ApiAction.Create, ApiResource.Roles),
        new("Update Roles", ApiAction.Update, ApiResource.Roles),
        new("Delete Roles", ApiAction.Delete, ApiResource.Roles),
        new("View RoleClaims", ApiAction.View, ApiResource.RoleClaims),
        new("Update RoleClaims", ApiAction.Update, ApiResource.RoleClaims),
        new("View Products", ApiAction.View, ApiResource.Products, true),
        new("Search Products", ApiAction.Search, ApiResource.Products, true),
        new("Create Products", ApiAction.Create, ApiResource.Products),
        new("Update Products", ApiAction.Update, ApiResource.Products),
        new("Delete Products", ApiAction.Delete, ApiResource.Products),
        new("Export Products", ApiAction.Export, ApiResource.Products),
        new("View Brands", ApiAction.View, ApiResource.Brands, true),
        new("Search Brands", ApiAction.Search, ApiResource.Brands, true),
        new("Create Brands", ApiAction.Create, ApiResource.Brands),
        new("Update Brands", ApiAction.Update, ApiResource.Brands),
        new("Delete Brands", ApiAction.Delete, ApiResource.Brands),
        new("Generate Brands", ApiAction.Generate, ApiResource.Brands),
        new("Clean Brands", ApiAction.Clean, ApiResource.Brands),
        new("View Categories", ApiAction.View, ApiResource.Categories, true),
        new("Search Categories", ApiAction.Search, ApiResource.Categories, true),
        new("Create Categories", ApiAction.Create, ApiResource.Categories),
        new("Update Categories", ApiAction.Update, ApiResource.Categories),
        new("Delete Categories", ApiAction.Delete, ApiResource.Categories),
        new("Generate Categories", ApiAction.Generate, ApiResource.Categories),
        new("Clean Categories", ApiAction.Clean, ApiResource.Categories),
        new("View Courses", ApiAction.View, ApiResource.Courses, true),
        new("Search Courses", ApiAction.Search, ApiResource.Courses, true),
        new("Create Courses", ApiAction.Create, ApiResource.Courses),
        new("Update Courses", ApiAction.Update, ApiResource.Courses),
        new("Delete Courses", ApiAction.Delete, ApiResource.Courses),
        new("Generate Courses", ApiAction.Generate, ApiResource.Courses),
        new("Clean Courses", ApiAction.Clean, ApiResource.Courses),
        new("View Chapters", ApiAction.View, ApiResource.Chapters, true),
        new("Search Chapters", ApiAction.Search, ApiResource.Chapters, true),
        new("Create Chapters", ApiAction.Create, ApiResource.Chapters),
        new("Update Chapters", ApiAction.Update, ApiResource.Chapters),
        new("Delete Chapters", ApiAction.Delete, ApiResource.Chapters),
        new("Generate Chapters", ApiAction.Generate, ApiResource.Chapters),
        new("Clean Chapters", ApiAction.Clean, ApiResource.Chapters),
        new("View Lessons", ApiAction.View, ApiResource.Lessons, true),
        new("Search Lessons", ApiAction.Search, ApiResource.Lessons, true),
        new("Create Lessons", ApiAction.Create, ApiResource.Lessons),
        new("Update Lessons", ApiAction.Update, ApiResource.Lessons),
        new("Delete Lessons", ApiAction.Delete, ApiResource.Lessons),
        new("Generate Lessons", ApiAction.Generate, ApiResource.Lessons),
        new("Clean Lessons", ApiAction.Clean, ApiResource.Lessons),
        new("View Absences", ApiAction.View, ApiResource.Absences, true),
        new("Search Absences", ApiAction.Search, ApiResource.Absences, true),
        new("Create Absences", ApiAction.Create, ApiResource.Absences),
        new("Update Absences", ApiAction.Update, ApiResource.Absences),
        new("Delete Absences", ApiAction.Delete, ApiResource.Absences),
        new("Generate Absences", ApiAction.Generate, ApiResource.Absences),
        new("Clean Absences", ApiAction.Clean, ApiResource.Absences),
        new("View Tenants", ApiAction.View, ApiResource.Tenants, IsRoot: true),
        new("Create Tenants", ApiAction.Create, ApiResource.Tenants, IsRoot: true),
        new("Update Tenants", ApiAction.Update, ApiResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", ApiAction.UpgradeSubscription, ApiResource.Tenants, IsRoot: true)
    };

    public static IReadOnlyList<ApiPermission> All { get; } = new ReadOnlyCollection<ApiPermission>(AllClaims);

    public static IReadOnlyList<ApiPermission> Root { get; } =
        new ReadOnlyCollection<ApiPermission>(AllClaims.Where(p => p.IsRoot).ToArray());

    public static IReadOnlyList<ApiPermission> Admin { get; } =
        new ReadOnlyCollection<ApiPermission>(AllClaims.Where(p => !p.IsRoot).ToArray());

    public static IReadOnlyList<ApiPermission> Basic { get; } =
        new ReadOnlyCollection<ApiPermission>(AllClaims.Where(p => p.IsBasic).ToArray());
}
