export const defaultRoutes = [
  {
    path: "/",
    component: () => import("@/layouts/default.vue"),
    meta: {
      requireAuth: false,
    },
    children: [
      {
        name: "home",
        path: "",
        component: () => import("@/pages/home.vue"),
        meta: {
          title: "Home",
        },
      },
      {
        name: "unauthorized",
        path: "unauthorized",
        component: () => import("@/pages/admin/unauthorized.vue"),
      },
    ],
  },
  {
    name: "error",
    path: "/:pathMatch(.*)*",
    component: () => import("@/pages/[...error].vue"),
  },
];
