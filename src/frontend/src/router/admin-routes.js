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
        titleKey: "admin.dashboard.title"
      }
    },
    {
      name: "profile",
      path: "profile",
      component: () => import("@/views/profile/IndexPage.vue"),
      meta: {
        titleKey: "admin.profile.title"
      }
    },
    {
      name: "audits",
      path: "audits",
      component: () => import("@/views/audits/IndexPage.vue"),
      meta: {
        titleKey: "admin.audits.title"
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
          name: "list-tenants",
          path: "list",
          component: () => import("@/views/tenants/IndexPage.vue"),
          meta: {
            titleKey: "admin.tenants.title"
          }
        },
        {
          name: "new-tenant",
          path: "new",
          component: () => import("@/views/tenants/NewPage.vue"),
          meta: {
            titleKey: "admin.tenants.new"
          }
        },
        {
          name: "edit-tenant",
          path: "edit/:id",
          component: () => import("@/views/tenants/EditPage.vue"),
          meta: {
            titleKey: "admin.tenants.edit"
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
          name: "list-roles",
          path: "list",
          component: () => import("@/views/roles/IndexPage.vue"),
          meta: {
            titleKey: "admin.roles.list"
          }
        },
        {
          name: "new-role",
          path: "new",
          component: () => import("@/views/roles/NewPage.vue"),
          meta: {
            titleKey: "admin.roles.new"
          }
        },
        {
          name: "edit-role",
          path: "edit/:id",
          component: () => import("@/views/roles/EditPage.vue"),
          meta: {
            titleKey: "admin.roles.edit"
          }
        }
      ]
    },
    {
      name: "UsersPage",
      path: "users",
      children: [
        {
          path: "",
          name: "default",
          redirect: "list"
        },
        {
          name: "list-users",
          path: "list",
          component: () => import("@/views/users/IndexPage.vue"),
          meta: {
            titleKey: "admin.users.list"
          }
        }
        // {
        //   name: "new-role",
        //   path: "new",
        //   component: () => import("@/views/roles/NewPage.vue"),
        //   meta: {
        //     titleKey: "admin.roles.new"
        //   }
        // },
        // {
        //   name: "edit-role",
        //   path: "edit/:id",
        //   component: () => import("@/views/roles/EditPage.vue"),
        //   meta: {
        //     titleKey: "admin.roles.edit"
        //   }
        // }
      ]
    }
  ]
};

export default AdminRoutes;
