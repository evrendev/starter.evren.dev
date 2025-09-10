export interface BasicUser {
  id?: string;
  gender?: string;
  email?: string;
  language?: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  initial?: string;
  phoneNumber?: string;
  birthday?: string;
  placeOfBirth?: string;
}
export interface User extends BasicUser {
  twoFactorEnabled: true | false;
  permissions?: string[];
}
export interface Permission {
  id: string;
  name: string;
}
export class Permissions {
  public static readonly DashboardView: string = "Permissions.Dashboard.View";
  public static readonly HangfireView: string = "Permissions.Hangfire.View";
  public static readonly UserView: string = "Permissions.Users.View";
  public static readonly UserSearch: string = "Permissions.Users.Search";
  public static readonly UserCreate: string = "Permissions.Users.Create";
  public static readonly UserUpdate: string = "Permissions.Users.Update";
  public static readonly UserDelete: string = "Permissions.Users.Delete";
  public static readonly UserExport: string = "Permissions.Users.Export";
  public static readonly UserRolesView: string = "Permissions.UserRoles.View";
  public static readonly UserRolesUpdate: string =
    "Permissions.UserRoles.Update";
  public static readonly RoleView: string = "Permissions.Roles.View";
  public static readonly RoleCreate: string = "Permissions.Roles.Create";
  public static readonly RoleUpdate: string = "Permissions.Roles.Update";
  public static readonly RoleDelete: string = "Permissions.Roles.Delete";
  public static readonly RoleClaimsView: string = "Permissions.RoleClaims.View";
  public static readonly RoleClaimsUpdate: string =
    "Permissions.RoleClaims.Update";
  public static readonly ProductView: string = "Permissions.Products.View";
  public static readonly ProductSearch: string = "Permissions.Products.Search";
  public static readonly ProductCreate: string = "Permissions.Products.Create";
  public static readonly ProductUpdate: string = "Permissions.Products.Update";
  public static readonly ProductDelete: string = "Permissions.Products.Delete";
  public static readonly ProductExport: string = "Permissions.Products.Export";
  public static readonly BrandView: string = "Permissions.Brands.View";
  public static readonly BrandSearch: string = "Permissions.Brands.Search";
  public static readonly BrandCreate: string = "Permissions.Brands.Create";
  public static readonly BrandUpdate: string = "Permissions.Brands.Update";
  public static readonly BrandDelete: string = "Permissions.Brands.Delete";
  public static readonly BrandGenerate: string = "Permissions.Brands.Generate";
  public static readonly BrandClean: string = "Permissions.Brands.Clean";
  public static readonly AbsenceView: string = "Permissions.Absences.View";
  public static readonly AbsenceSearch: string = "Permissions.Absences.Search";
  public static readonly AbsenceCreate: string = "Permissions.Absences.Create";
  public static readonly AbsenceUpdate: string = "Permissions.Absences.Update";
  public static readonly AbsenceDelete: string = "Permissions.Absences.Delete";
  public static readonly AbsenceGenerate: string =
    "Permissions.Absences.Generate";
  public static readonly AbsenceClean: string = "Permissions.Absences.Clean";
  public static readonly TenantView: string = "Permissions.Tenants.View";
  public static readonly TenantCreate: string = "Permissions.Tenants.Create";
  public static readonly TenantUpdate: string = "Permissions.Tenants.Update";
  public static readonly TenantUpgradeSubscription: string =
    "Permissions.Tenants.UpgradeSubscription";

  public static all(): string[] {
    return [
      this.DashboardView,
      this.HangfireView,
      this.UserView,
      this.UserSearch,
      this.UserCreate,
      this.UserUpdate,
      this.UserDelete,
      this.UserExport,
      this.UserRolesView,
      this.UserRolesUpdate,
      this.RoleView,
      this.RoleCreate,
      this.RoleUpdate,
      this.RoleDelete,
      this.RoleClaimsView,
      this.RoleClaimsUpdate,
      this.ProductView,
      this.ProductSearch,
      this.ProductCreate,
      this.ProductUpdate,
      this.ProductDelete,
      this.ProductExport,
      this.BrandView,
      this.BrandSearch,
      this.BrandCreate,
      this.BrandUpdate,
      this.BrandDelete,
      this.BrandGenerate,
      this.BrandClean,
      this.AbsenceView,
      this.AbsenceSearch,
      this.AbsenceCreate,
      this.AbsenceUpdate,
      this.AbsenceDelete,
      this.AbsenceGenerate,
      this.AbsenceClean,
      this.TenantView,
      this.TenantCreate,
      this.TenantUpdate,
      this.TenantUpgradeSubscription,
    ];
  }
}
