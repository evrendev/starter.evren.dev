import { Permissions } from "@/models/user";

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
          requiresPermission: Permissions.DashboardView,
        },
      },
      {
        name: "tenants",
        path: "tenants",
        children: [
          {
            name: "tesnant-list",
            path: "",
            component: async () =>
              await import("@/pages/admin/tenants/index.vue"),
            meta: {
              requiresPermission: Permissions.TenantView,
              title: "admin.tenants.list.title",
            },
          },
          {
            name: "tenant-create",
            path: "create",
            component: async () =>
              await import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: Permissions.TenantCreate,
              title: "admin.tenants.create.title",
            },
          },
          {
            name: "tenant-view",
            path: ":id/view",
            component: async () =>
              await import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: Permissions.TenantView,
              title: "admin.tenants.view.title",
            },
          },
          {
            name: "tenant-edit",
            path: ":id/edit",
            component: async () =>
              await import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: Permissions.TenantUpdate,
              title: "admin.tenants.edit.title",
            },
          },
        ],
      },
      {
        name: "roles",
        path: "roles",
        component: () => import("@/pages/admin/roles.vue"),
        meta: {
          requiresPermission: Permissions.RoleView,
        },
      },
      {
        name: "users",
        path: "users",
        component: () => import("@/pages/admin/users.vue"),
        meta: {
          requiresPermission: Permissions.UserView,
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
