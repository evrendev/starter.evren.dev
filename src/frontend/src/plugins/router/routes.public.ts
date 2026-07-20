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
    name: "chapter-player",
    path: "/chapters/:id/player",
    component: () => import("@/pages/chapter-player/index.vue"),
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
