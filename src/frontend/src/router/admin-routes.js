const AdminRoutes = {
  path: "/admin",
  meta: {
    requiresAuth: true
  },
  component: () => import("@/layouts/full/FullLayout.vue"),
  children: [
    {
      name: "home",
      path: "",
      component: () => import("@/views/dashboards/IndexPage.vue")
    },
    {
      name: "audits",
      path: "audits",
      component: () => import("@/views/audits/IndexPage.vue")
    },
    {
      name: "TenantsPage",
      path: "tenants",
      children: [
        {
          name: "tenants",
          path: "",
          component: () => import("@/views/tenants/IndexPage.vue")
        },
        {
          name: "new-tenant",
          path: "new",
          component: () => import("@/views/tenants/NewPage.vue")
        },
        {
          name: "edit-tenant",
          path: "edit/:id",
          component: () => import("@/views/tenants/NewPage.vue")
        }
      ]
    }
  ]
};

export default AdminRoutes;
