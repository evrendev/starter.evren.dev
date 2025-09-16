export const authRoutes = [
  {
    path: "/auth",
    component: () => import("@/layouts/auth.vue"),
    meta: {
      requireAuth: false,
    },
    children: [
      {
        name: "login",
        path: "login",
        component: () => import("@/pages/auth/login.vue"),
      },
      {
        name: "forgot-password",
        path: "forgot-password",
        component: () => import("@/pages/auth/forgot-password.vue"),
      },
      {
        name: "reset-password",
        path: "reset-password",
        component: () => import("@/pages/auth/reset-password.vue"),
      },
    ],
  },
];
