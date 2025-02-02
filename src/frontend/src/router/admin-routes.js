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
      component: () => import("@/views/dashboards/IndexPage.vue"),
      meta: {
        titleKey: "pages.titles.dashboard"
      }
    },
    {
      name: "audits",
      path: "audits",
      component: () => import("@/views/audits/IndexPage.vue"),
      meta: {
        titleKey: "pages.titles.auditLogs"
      }
    },
    {
      name: "TenantsPage",
      path: "tenants",
      children: [
        {
          path: "",
          name: "default",
          redirect: "list"
        },
        {
          name: "tenants",
          path: "list",
          component: () => import("@/views/tenants/IndexPage.vue"),
          meta: {
            titleKey: "pages.titles.tenants"
          }
        },
        {
          name: "new-tenant",
          path: "new",
          component: () => import("@/views/tenants/NewPage.vue"),
          meta: {
            titleKey: "pages.titles.newTenant"
          }
        },
        {
          name: "edit-tenant",
          path: "edit/:id",
          component: () => import("@/views/tenants/EditPage.vue"),
          meta: {
            titleKey: "pages.titles.editTenant"
          }
        }
      ]
    }
  ]
};

export default AdminRoutes;
