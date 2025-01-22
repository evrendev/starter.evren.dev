const PanelRoutes = {
  path: "/dashboard",
  meta: {
    requiresAuth: true
  },
  component: () => import("@/layouts/full/FullLayout.vue"),
  children: [
    {
      name: "StarterPage",
      path: "",
      component: () => import("@/views/StarterPage.vue")
    }
  ]
};

export default PanelRoutes;
