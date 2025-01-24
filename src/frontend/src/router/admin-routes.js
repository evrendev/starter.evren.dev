const AdminRoutes = {
  path: "/admin",
  meta: {
    requiresAuth: true
  },
  component: () => import("@/layouts/full/FullLayout.vue"),
  children: [
    {
      name: "StarterPage",
      path: "",
      component: () => import("@/views/dashboards/IndexPage.vue")
    },
    {
      name: "AuditsPage",
      path: "audits",
      component: () => import("@/views/audits/IndexPage.vue")
    }
  ]
};

export default AdminRoutes;
