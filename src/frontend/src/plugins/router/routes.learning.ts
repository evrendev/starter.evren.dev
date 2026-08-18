export const learningRoutes = [
  {
    path: "/learning",
    component: () => import("@/layouts/admin.vue"),
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        name: "learning-catalog",
        path: "catalog",
        component: () => import("@/pages/learning/catalog.vue"),
        meta: {
          title: "learning.catalog.title",
        },
      },
      {
        name: "learning-my-courses",
        path: "my-courses",
        component: () => import("@/pages/learning/my-courses.vue"),
        meta: {
          title: "learning.myCourses.title",
        },
      },
      {
        name: "learning-checkout-return",
        path: "checkout-return",
        component: () => import("@/pages/learning/checkout-return.vue"),
        meta: {
          title: "learning.checkoutReturn.title",
        },
      },
    ],
  },
];
