export interface BasicUser {
  id?: string;
  email?: string;
  fullName?: string;
  initial?: string;
  isActive: boolean;
  twoFactorEnabled: boolean;
}

export interface User extends BasicUser {
  gender?: number;
  language?: number;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  birthday?: string;
  placeOfBirth?: string;
}

export interface UserWithPermission extends User {
  permissions?: string[];
}

export interface Permission {
  id: string;
  name: string;
}

export class Permissions {
  // General permissions
  public static readonly DashboardView: string = "Permissions.Dashboard.View";
  public static readonly HangfireView: string = "Permissions.Hangfire.View";

  // User permissions
  public static readonly UserView: string = "Permissions.Users.View";
  public static readonly UserSearch: string = "Permissions.Users.Search";
  public static readonly UserCreate: string = "Permissions.Users.Create";
  public static readonly UserUpdate: string = "Permissions.Users.Update";
  public static readonly UserDelete: string = "Permissions.Users.Delete";
  public static readonly UserExport: string = "Permissions.Users.Export";

  // UserRoles permissions
  public static readonly UserRolesView: string = "Permissions.UserRoles.View";
  public static readonly UserRolesUpdate: string =
    "Permissions.UserRoles.Update";

  // Role permissions
  public static readonly RoleView: string = "Permissions.Roles.View";
  public static readonly RoleSearch: string = "Permissions.Roles.Search";
  public static readonly RoleCreate: string = "Permissions.Roles.Create";
  public static readonly RoleUpdate: string = "Permissions.Roles.Update";
  public static readonly RoleDelete: string = "Permissions.Roles.Delete";
  private static readonly RoleExport: string = "Permissions.Roles.Export";

  // RoleClaim permissions
  public static readonly RoleClaimsView: string = "Permissions.RoleClaims.View";
  public static readonly RoleClaimsUpdate: string =
    "Permissions.RoleClaims.Update";

  // Product permissions
  public static readonly ProductView: string = "Permissions.Products.View";
  public static readonly ProductSearch: string = "Permissions.Products.Search";
  public static readonly ProductCreate: string = "Permissions.Products.Create";
  public static readonly ProductUpdate: string = "Permissions.Products.Update";
  public static readonly ProductDelete: string = "Permissions.Products.Delete";
  public static readonly ProductExport: string = "Permissions.Products.Export";

  // Brand permissions
  public static readonly BrandView: string = "Permissions.Brands.View";
  public static readonly BrandSearch: string = "Permissions.Brands.Search";
  public static readonly BrandCreate: string = "Permissions.Brands.Create";
  public static readonly BrandUpdate: string = "Permissions.Brands.Update";
  public static readonly BrandDelete: string = "Permissions.Brands.Delete";
  public static readonly BrandGenerate: string = "Permissions.Brands.Generate";
  public static readonly BrandClean: string = "Permissions.Brands.Clean";

  // Absence permissions
  public static readonly AbsenceView: string = "Permissions.Absences.View";
  public static readonly AbsenceSearch: string = "Permissions.Absences.Search";
  public static readonly AbsenceCreate: string = "Permissions.Absences.Create";
  public static readonly AbsenceUpdate: string = "Permissions.Absences.Update";
  public static readonly AbsenceDelete: string = "Permissions.Absences.Delete";
  public static readonly AbsenceGenerate: string =
    "Permissions.Absences.Generate";
  public static readonly AbsenceClean: string = "Permissions.Absences.Clean";

  // Category permissions
  public static readonly CategoryView: string = "Permissions.Categories.View";
  public static readonly CategorySearch: string =
    "Permissions.Categories.Search";
  public static readonly CategoryCreate: string =
    "Permissions.Categories.Create";
  public static readonly CategoryUpdate: string =
    "Permissions.Categories.Update";
  public static readonly CategoryDelete: string =
    "Permissions.Categories.Delete";
  public static readonly CategoryGenerate: string =
    "Permissions.Categories.Generate";
  public static readonly CategoryClean: string = "Permissions.Categories.Clean";

  // Course permissions
  public static readonly CourseView: string = "Permissions.Courses.View";
  public static readonly CourseSearch: string = "Permissions.Courses.Search";
  public static readonly CourseCreate: string = "Permissions.Courses.Create";
  public static readonly CourseUpdate: string = "Permissions.Courses.Update";
  public static readonly CourseDelete: string = "Permissions.Courses.Delete";
  public static readonly CourseGenerate: string =
    "Permissions.Courses.Generate";
  public static readonly CourseClean: string = "Permissions.Courses.Clean";

  // Chapter permissions
  public static readonly ChapterView: string = "Permissions.Chapters.View";
  public static readonly ChapterSearch: string = "Permissions.Chapters.Search";
  public static readonly ChapterCreate: string = "Permissions.Chapters.Create";
  public static readonly ChapterUpdate: string = "Permissions.Chapters.Update";
  public static readonly ChapterDelete: string = "Permissions.Chapters.Delete";
  public static readonly ChapterGenerate: string =
    "Permissions.Chapters.Generate";
  public static readonly ChapterClean: string = "Permissions.Chapters.Clean";

  // Lesson permissions
  public static readonly LessonView: string = "Permissions.Lessons.View";
  public static readonly LessonSearch: string = "Permissions.Lessons.Search";
  public static readonly LessonCreate: string = "Permissions.Lessons.Create";
  public static readonly LessonUpdate: string = "Permissions.Lessons.Update";
  public static readonly LessonDelete: string = "Permissions.Lessons.Delete";
  public static readonly LessonGenerate: string =
    "Permissions.Lessons.Generate";
  public static readonly LessonClean: string = "Permissions.Lessons.Clean";

  // Tenant permissions
  public static readonly TenantView: string = "Permissions.Tenants.View";
  public static readonly TenantSearch: string = "Permissions.Tenants.Search";
  public static readonly TenantCreate: string = "Permissions.Tenants.Create";
  public static readonly TenantUpdate: string = "Permissions.Tenants.Update";
  public static readonly TenantDelete: string = "Permissions.Tenants.Delete";
  public static readonly TenantUpgradeSubscription: string =
    "Permissions.Tenants.UpgradeSubscription";
}

export interface Log {
  id: string;
  userId: string;
  type: string;
  tableName: string;
  dateTime: Date;
  oldValues: string;
  newValues: string;
  affectedColumns: string;
  primaryKey: string;
}
