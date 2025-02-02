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
    },
    {
      name: "RolesPage",
      path: "roles",
      children: [
        {
          path: "",
          name: "default",
          redirect: "list"
        },
        {
          name: "roles",
          path: "list",
          component: () => import("@/views/roles/IndexPage.vue"),
          meta: {
            titleKey: "pages.titles.roles"
          }
        },
        {
          name: "new-role",
          path: "new",
          component: () => import("@/views/roles/NewPage.vue"),
          meta: {
            titleKey: "pages.titles.newRole"
          }
        },
        {
          name: "edit-role",
          path: "edit/:id",
          component: () => import("@/views/roles/EditPage.vue"),
          meta: {
            titleKey: "pages.titles.editRole"
          }
        }
      ]
    }
  ]
};

export default AdminRoutes;
