export const routes = [
  {
    path: "/",
    component: () => import("@/layouts/default.vue"),
    meta: {
      requiresGuest: true,
    },
    children: [
      {
        name: "home",
        path: "home",
        component: () => import("@/pages/home.vue"),
        meta: {
          title: "Home",
        },
      },
    ],
  },
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
        component: () => import("@/pages/dashboard.vue"),
        meta: {
          requiresPermission: "Permissions.Dashboard.View",
        },
      },
      {
        name: "account-settings",
        path: "account-settings",
        component: () => import("@/pages/account-settings.vue"),
      },
      {
        name: "typography",
        path: "typography",
        component: () => import("@/pages/typography.vue"),
      },
      {
        name: "icons",
        path: "icons",
        component: () => import("@/pages/icons.vue"),
      },
      {
        name: "cards",
        path: "cards",
        component: () => import("@/pages/cards.vue"),
      },
      {
        name: "tables",
        path: "tables",
        component: () => import("@/pages/tables.vue"),
      },
      {
        name: "form-layouts",
        path: "form-layouts",
        component: () => import("@/pages/form-layouts.vue"),
      },
      {
        name: "unauthorized",
        path: "unauthorized",
        component: () => import("@/pages/unauthorized.vue"),
      },
    ],
  },
  {
    path: "/auth",
    component: () => import("@/layouts/auth.vue"),
    meta: {
      requiresAuth: false,
    },
    children: [
      {
        name: "login",
        path: "login",
        component: () => import("@/pages/login.vue"),
      },
      {
        name: "forgot-password",
        path: "forgot-password",
        component: () => import("@/pages/forgot-password.vue"),
      },
      {
        name: "reset-password",
        path: "reset-password",
        component: () => import("@/pages/reset-password.vue"),
      },
      {
        name: "error",
        path: "/:pathMatch(.*)*",
        component: () => import("@/pages/[...error].vue"),
      },
    ],
  },
];
