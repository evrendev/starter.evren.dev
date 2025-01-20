const MainRoutes = {
  path: "/dashboard",
  meta: {
    requiresAuth: true
  },
  redirect: "/dashboard/",
  component: () => import("@/layouts/full/FullLayout.vue"),
  children: [
    {
      name: "StarterPage",
      path: "home",
      component: () => import("@/views/StarterPage.vue")
    }
  ]
};

export default MainRoutes;
