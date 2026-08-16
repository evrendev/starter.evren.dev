using System.Collections.ObjectModel;

namespace EvrenDev.Shared.Authorization;

public static class ApiRoles
{
    public const string Admin = nameof(Admin);
    // Content-management role: full CRUD on the LMS catalog (Categories/Courses/
    // Chapters/Pages/Notes), no access to Users/Roles/RoleClaims/Tenants/UserRoles
    public const string Editor = nameof(Editor);
    // Formerly "Basic" — renamed for clarity now that self-register (Task O1)
    // makes this the role every new learner actually lands in
    public const string Student = nameof(Student);

    public static IReadOnlyList<string> DefaultRoles { get; } =
        new ReadOnlyCollection<string>(new[] { Admin, Editor, Student });

    public static bool IsDefault(string roleName)
    {
        return DefaultRoles.Any(r => r == roleName);
    }
}
