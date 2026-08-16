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
        name: "register",
        path: "register",
        component: () => import("@/pages/auth/register.vue"),
      },
      {
        name: "confirm-email",
        path: "confirm-email",
        component: () => import("@/pages/auth/confirm-email.vue"),
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
