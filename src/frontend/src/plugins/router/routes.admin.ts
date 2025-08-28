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
        name: "account-settings",
        path: "account-settings",
        component: () => import("@/pages/admin/account-settings.vue"),
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
