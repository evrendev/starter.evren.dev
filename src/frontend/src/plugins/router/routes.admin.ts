import { Permissions } from "@/models/user";

export const adminRoutes = [
  {
    path: "/admin",
    component: async () => await import("@/layouts/admin.vue"),
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        name: "dashboard",
        path: "",
        component: async () => await import("@/pages/admin/dashboard.vue"),
        meta: {
          requiresPermission: Permissions.DashboardView,
        },
      },
      {
        name: "tenants",
        path: "tenants",
        children: [
          {
            name: "tenant-list",
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
        children: [
          {
            name: "role-list",
            path: "",
            component: async () =>
              await import("@/pages/admin/roles/index.vue"),
            meta: {
              requiresPermission: Permissions.RoleView,
              title: "admin.roles.list.title",
            },
          },
          {
            name: "role-create",
            path: "create",
            component: async () => await import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: Permissions.RoleCreate,
              title: "admin.roles.create.title",
            },
          },
          {
            name: "role-view",
            path: ":id/view",
            component: async () => await import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: Permissions.RoleView,
              title: "admin.roles.view.title",
            },
          },
          {
            name: "role-edit",
            path: ":id/edit",
            component: async () => await import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: Permissions.RoleUpdate,
              title: "admin.roles.edit.title",
            },
          },
        ],
      },
      {
        name: "personal",
        path: "personal",
        redirect: { name: "personal-profile" },
        children: [
          {
            name: "personal-profile",
            path: "profile",
            component: async () =>
              await import("@/pages/admin/personal/profile.vue"),
            meta: {
              title: "admin.personal.profile.title",
            },
          },
          {
            name: "personal-security",
            path: "security",
            component: async () =>
              await import("@/pages/admin/personal/security.vue"),
            meta: {
              title: "admin.personal.security.title",
            },
          },
          {
            name: "personal-logs",
            path: "logs",
            component: async () =>
              await import("@/pages/admin/personal/logs.vue"),
            meta: {
              title: "admin.personal.logs.title",
            },
          },
        ],
      },
      {
        name: "users",
        path: "users",
        component: async () => await import("@/pages/admin/users.vue"),
        meta: {
          requiresPermission: Permissions.UserView,
        },
      },
      {
        name: "typography",
        path: "typography",
        component: async () => await import("@/pages/admin/typography.vue"),
      },
      {
        name: "icons",
        path: "icons",
        component: async () => await import("@/pages/admin/icons.vue"),
      },
      {
        name: "cards",
        path: "cards",
        component: async () => await import("@/pages/admin/cards.vue"),
      },
      {
        name: "tables",
        path: "tables",
        component: async () => await import("@/pages/admin/tables.vue"),
      },
      {
        name: "form-layouts",
        path: "form-layouts",
        component: async () => await import("@/pages/admin/form-layouts.vue"),
      },
      {
        name: "unauthorized",
        path: "unauthorized",
        component: async () => await import("@/pages/admin/unauthorized.vue"),
      },
    ],
  },
];
