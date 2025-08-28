export const adminRoutes = [
  {
    path: "/admin",
    component: () => import("@/layouts/admin.vue"),
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        name: "dashboard",
        path: "",
        component: () => import("@/pages/admin/dashboard.vue"),
        meta: {
          requiresPermission: "Permissions.Dashboard.View",
        },
      },
      {
        name: "tenants",
        path: "tenants",
        component: () => import("@/pages/admin/tenants.vue"),
        meta: {
          requiresPermission: "Permissions.Tenants.View",
        },
      },
      {
        name: "roles",
        path: "roles",
        component: () => import("@/pages/admin/roles.vue"),
        meta: {
          requiresPermission: "Permissions.Roles.View",
        },
      },
      {
        name: "users",
        path: "users",
        component: () => import("@/pages/admin/users.vue"),
        meta: {
          requiresPermission: "Permissions.Users.View",
        },
      },
      {
        name: "profile",
        path: "profile",
        component: () => import("@/pages/admin/profile.vue"),
      },
      {
        name: "typography",
        path: "typography",
        component: () => import("@/pages/admin/typography.vue"),
      },
      {
        name: "icons",
        path: "icons",
        component: () => import("@/pages/admin/icons.vue"),
      },
      {
        name: "cards",
        path: "cards",
        component: () => import("@/pages/admin/cards.vue"),
      },
      {
        name: "tables",
        path: "tables",
        component: () => import("@/pages/admin/tables.vue"),
      },
      {
        name: "form-layouts",
        path: "form-layouts",
        component: () => import("@/pages/admin/form-layouts.vue"),
      },
      {
        name: "unauthorized",
        path: "unauthorized",
        component: () => import("@/pages/admin/unauthorized.vue"),
      },
    ],
  },
];
