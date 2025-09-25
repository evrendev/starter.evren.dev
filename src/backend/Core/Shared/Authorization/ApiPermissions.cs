using System.Collections.ObjectModel;

namespace EvrenDev.Shared.Authorization;

public static class ApiPermissions
{
    private static readonly ApiPermission[] AllClaims =
    {
        // General permissions
        new("View Dashboard", ApiAction.View, ApiResource.Dashboard),
        new("View Hangfire", ApiAction.View, ApiResource.Hangfire),
        // User permissions
        new("View Users", ApiAction.View, ApiResource.Users),
        new("Search Users", ApiAction.Search, ApiResource.Users),
        new("Create Users", ApiAction.Create, ApiResource.Users),
        new("Update Users", ApiAction.Update, ApiResource.Users),
        new("Delete Users", ApiAction.Delete, ApiResource.Users),
        new("Export Users", ApiAction.Export, ApiResource.Users),
        // UserRoles permissions
        new("View UserRoles", ApiAction.View, ApiResource.UserRoles),
        new("Update UserRoles", ApiAction.Update, ApiResource.UserRoles),
        // Role permissions
        new("View Roles", ApiAction.View, ApiResource.Roles),
        new("Create Roles", ApiAction.Create, ApiResource.Roles),
        new("Update Roles", ApiAction.Update, ApiResource.Roles),
        new("Delete Roles", ApiAction.Delete, ApiResource.Roles),
        new("Export Roles", ApiAction.Export, ApiResource.Roles),
        new("Search Roles", ApiAction.Search, ApiResource.Roles),
        // RoleClaim permissions
        new("View RoleClaims", ApiAction.View, ApiResource.RoleClaims),
        new("Update RoleClaims", ApiAction.Update, ApiResource.RoleClaims),
        // Product permissions
        new("View Products", ApiAction.View, ApiResource.Products, IsBasic: true),
        new("Search Products", ApiAction.Search, ApiResource.Products, IsBasic: true),
        new("Create Products", ApiAction.Create, ApiResource.Products),
        new("Update Products", ApiAction.Update, ApiResource.Products),
        new("Delete Products", ApiAction.Delete, ApiResource.Products),
        new("Export Products", ApiAction.Export, ApiResource.Products),
        // Brand permissions
        new("View Brands", ApiAction.View, ApiResource.Brands, IsBasic: true),
        new("Search Brands", ApiAction.Search, ApiResource.Brands, IsBasic: true),
        new("Create Brands", ApiAction.Create, ApiResource.Brands),
        new("Update Brands", ApiAction.Update, ApiResource.Brands),
        new("Delete Brands", ApiAction.Delete, ApiResource.Brands),
        new("Generate Brands", ApiAction.Generate, ApiResource.Brands),
        new("Export Brands", ApiAction.Export, ApiResource.Brands),
        new("Clean Brands", ApiAction.Clean, ApiResource.Brands),
        // Category permissions
        new("View Categories", ApiAction.View, ApiResource.Categories, IsBasic: true),
        new("Search Categories", ApiAction.Search, ApiResource.Categories, IsBasic: true),
        new("Create Categories", ApiAction.Create, ApiResource.Categories),
        new("Update Categories", ApiAction.Update, ApiResource.Categories),
        new("Delete Categories", ApiAction.Delete, ApiResource.Categories),
        new("Export Categories", ApiAction.Export, ApiResource.Categories),
        new("Clean Categories", ApiAction.Clean, ApiResource.Categories),
        // Course permissions
        new("View Courses", ApiAction.View, ApiResource.Courses, IsBasic: true),
        new("Search Courses", ApiAction.Search, ApiResource.Courses, IsBasic: true),
        new("Create Courses", ApiAction.Create, ApiResource.Courses),
        new("Update Courses", ApiAction.Update, ApiResource.Courses),
        new("Delete Courses", ApiAction.Delete, ApiResource.Courses),
        new("Generate Courses", ApiAction.Generate, ApiResource.Courses),
        new("Export Courses", ApiAction.Export, ApiResource.Courses),
        new("Clean Courses", ApiAction.Clean, ApiResource.Courses),
        // Chapter permissions
        new("View Chapters", ApiAction.View, ApiResource.Chapters, IsBasic: true),
        new("Search Chapters", ApiAction.Search, ApiResource.Chapters, IsBasic: true),
        new("Create Chapters", ApiAction.Create, ApiResource.Chapters),
        new("Update Chapters", ApiAction.Update, ApiResource.Chapters),
        new("Delete Chapters", ApiAction.Delete, ApiResource.Chapters),
        new("Generate Chapters", ApiAction.Generate, ApiResource.Chapters),
        new("Export Chapters", ApiAction.Export, ApiResource.Chapters),
        new("Clean Chapters", ApiAction.Clean, ApiResource.Chapters),
        // Lesson permissions
        new("View Lessons", ApiAction.View, ApiResource.Lessons, IsBasic: true),
        new("Search Lessons", ApiAction.Search, ApiResource.Lessons, IsBasic:true),
        new("Create Lessons", ApiAction.Create, ApiResource.Lessons),
        new("Update Lessons", ApiAction.Update, ApiResource.Lessons),
        new("Delete Lessons", ApiAction.Delete, ApiResource.Lessons),
        new("Generate Lessons", ApiAction.Generate, ApiResource.Lessons),
        new("Export Lessons", ApiAction.Export, ApiResource.Lessons),
        new("Clean Lessons", ApiAction.Clean, ApiResource.Lessons),
        // Absence permissions
        new("View Absences", ApiAction.View, ApiResource.Absences, IsBasic: true),
        new("Search Absences", ApiAction.Search, ApiResource.Absences, IsBasic: true),
        new("Create Absences", ApiAction.Create, ApiResource.Absences),
        new("Update Absences", ApiAction.Update, ApiResource.Absences),
        new("Delete Absences", ApiAction.Delete, ApiResource.Absences),
        new("Generate Absences", ApiAction.Generate, ApiResource.Absences),
        new("Export Absences", ApiAction.Export, ApiResource.Absences),
        new("Clean Absences", ApiAction.Clean, ApiResource.Absences),
        // Tenant permissions are only for root users
        new("View Tenants", ApiAction.View, ApiResource.Tenants, IsRoot: true),
        new("Create Tenants", ApiAction.Create, ApiResource.Tenants, IsRoot: true),
        new("Update Tenants", ApiAction.Update, ApiResource.Tenants, IsRoot: true),
        new("Delete Tenants", ApiAction.Delete, ApiResource.Tenants, IsRoot: true),
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
