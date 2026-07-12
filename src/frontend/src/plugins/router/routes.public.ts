export const publicRoutes = [
  {
    path: "/",
    component: () => import("@/layouts/public.vue"),
    meta: {
      requireAuth: false,
    },
    children: [
      {
        name: "home",
        path: "",
        component: () => import("@/pages/public/home.vue"),
        meta: {
          title: "Home",
        },
      },
    ],
  },
  {
    name: "lesson-player",
    path: "/lessons/:id/player",
    component: () => import("@/pages/lesson-player/index.vue"),
    meta: {
      requiresAuth: true,
      layout: "blank",
    },
  },
  {
    name: "error",
    path: "/:pathMatch(.*)*",
    component: () => import("@/pages/[...error].vue"),
  },
];
