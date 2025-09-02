import { Permissions } from "@/models/user";
import { red } from "vuetify/util/colors";

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
        component: () => import("@/layouts/default.vue"),
        children: [
          {
            name: "tenants-list",
            path: "",
            component: () => import("@/pages/admin/tenants/index.vue"),
            meta: {
              requiresPermission: Permissions.TenantView,
            },
          },
          {
            name: "tenants-view",
            path: ":id/view",
            component: () => import("@/pages/admin/tenants/edit.vue"),
            meta: {
              requiresPermission: Permissions.TenantView,
            },
          },
          {
            name: "tenants-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/tenants/edit.vue"),
            meta: {
              requiresPermission: Permissions.TenantUpdate,
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
