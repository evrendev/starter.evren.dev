// @/constants/permissions.ts

/**
 * @description Defines all possible actions that can be performed on a resource.
 * Using 'as const' makes it a readonly type and allows us to derive a union type from its values.
 * This corresponds to 'ApiAction.cs' in your backend.
 */
export const ACTIONS = {
  VIEW: "View",
  SEARCH: "Search",
  CREATE: "Create",
  UPDATE: "Update",
  DELETE: "Delete",
  EXPORT: "Export",
  // GENERATE: "Generate",
  // CLEAN: "Clean",
  // UPGRADE_SUBSCRIPTION: "UpgradeSubscription",
} as const;

/**
 * @description Defines all resources in the system.
 * This corresponds to 'ApiResource.cs' in your backend.
 */
export const RESOURCES = {
  // DASHBOARD: "Dashboard",
  // HANGFIRE: "Hangfire",
  USERS: "Users",
  USER_ROLES: "UserRoles",
  ROLES: "Roles",
  ROLE_CLAIMS: "RoleClaims",
  PRODUCTS: "Products",
  BRANDS: "Brands",
  CATEGORIES: "Categories",
  COURSES: "Courses",
  CHAPTERS: "Chapters",
  LESSONS: "Lessons",
  ABSENCES: "Absences",
  // TENANTS: "Tenants",
} as const;

// --- Type Definitions for strong typing ---

/**
 * @description A union type representing all possible action strings.
 * e.g., 'View' | 'Create' | ...
 */
export type Action = (typeof ACTIONS)[keyof typeof ACTIONS];

/**
 * @description A union type representing all possible resource strings.
 * e.g., 'Users' | 'Roles' | ...
 */
export type Resource = (typeof RESOURCES)[keyof typeof RESOURCES];

/**
 * @description This is the key data structure for the UI.
 * It maps each resource to a list of actions that are available for it.
 * This structure makes it very easy to dynamically generate the permission matrix in the Vue component.
 * We are manually defining the relationships here, mirroring the 'AllClaims' array in your C# code.
 */
export const PERMISSION_MAP: Record<Resource, Action[]> = {
  // [RESOURCES.DASHBOARD]: [ACTIONS.VIEW],
  // [RESOURCES.HANGFIRE]: [ACTIONS.VIEW],
  [RESOURCES.USERS]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
  ],
  [RESOURCES.USER_ROLES]: [ACTIONS.VIEW, ACTIONS.UPDATE],
  [RESOURCES.ROLES]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
  ],
  [RESOURCES.ROLE_CLAIMS]: [ACTIONS.VIEW, ACTIONS.UPDATE],
  [RESOURCES.PRODUCTS]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
  ],
  [RESOURCES.BRANDS]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.GENERATE,
    // ACTIONS.CLEAN,
  ],
  [RESOURCES.CATEGORIES]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.CLEAN,
  ],
  [RESOURCES.COURSES]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.GENERATE,
    // ACTIONS.CLEAN,
  ],
  [RESOURCES.CHAPTERS]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.GENERATE,
    // ACTIONS.CLEAN,
  ],
  [RESOURCES.LESSONS]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.GENERATE,
    // ACTIONS.CLEAN,
  ],
  [RESOURCES.ABSENCES]: [
    ACTIONS.VIEW,
    ACTIONS.SEARCH,
    ACTIONS.CREATE,
    ACTIONS.UPDATE,
    ACTIONS.DELETE,
    ACTIONS.EXPORT,
    // ACTIONS.GENERATE,
    // ACTIONS.CLEAN,
  ],
  // [RESOURCES.TENANTS]: [
  //   ACTIONS.VIEW,
  //   ACTIONS.SEARCH,
  //   ACTIONS.CREATE,
  //   ACTIONS.UPDATE,
  //   ACTIONS.DELETE,
  //   ACTIONS.UPGRADE_SUBSCRIPTION,
  // ],
};

/**
 * @description A flattened list of all unique actions available across all resources.
 * This is useful for creating the table headers dynamically.
 */
export const ALL_UNIQUE_ACTIONS = Object.values(ACTIONS);
