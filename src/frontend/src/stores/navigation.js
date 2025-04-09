import { defineStore } from "pinia";
import { markRaw } from "vue";
import { DashboardIcon, ListCheckIcon, UsersIcon, Category2Icon, GitCompareIcon, FileBarcodeIcon, CoinEuroIcon } from "vue-tabler-icons";
import { useAuthStore } from "./auth";

const DONATIONS_PERMISSIONS = ["Donations.Read", "Donations.Create", "Donations.Edit", "Donations.Delete"];
const TODOS_PERMISSIONS = ["Todos.Read", "Todos.Create", "Todos.Edit", "Todos.Delete"];
const USERS_PERMISSIONS = ["Users.Read", "Users.Create", "Users.Edit", "Users.Delete"];
const ROLES_PERMISSIONS = ["Roles.Read", "Roles.Create", "Roles.Edit", "Roles.Delete"];
const TENANTS_PERMISSIONS = ["Tenants.Read", "Tenants.Create", "Tenants.Edit", "Tenants.Delete"];
const AUDITS_PERMISSIONS = ["Audits.Read", "Audits.Create", "Audits.Edit", "Audits.Delete"];

const ADMIN_PERMISSIONS = [...USERS_PERMISSIONS, ...ROLES_PERMISSIONS, ...TENANTS_PERMISSIONS, ...AUDITS_PERMISSIONS];

// Mark icon components as raw to prevent reactivity
const icons = {
  dashboard: markRaw(DashboardIcon),
  donations: markRaw(CoinEuroIcon),
  todos: markRaw(ListCheckIcon),
  users: markRaw(UsersIcon),
  roles: markRaw(Category2Icon),
  tenants: markRaw(GitCompareIcon),
  audits: markRaw(FileBarcodeIcon)
};

export const useNavigationStore = defineStore("navigation", {
  state: () => ({
    items: []
  }),
  getters: {
    sidebarItems: (state) => state.items
  },
  actions: {
    hasPermission(permission) {
      const authStore = useAuthStore();
      return authStore.user?.permissions?.includes(permission);
    },
    hasAnyPermission(permissions) {
      return permissions.some((permission) => this.hasPermission(permission));
    },
    generateSidebarItems() {
      const items = [
        { header: "components.sidebar.dashboard.header" },
        {
          title: "components.sidebar.dashboard.title",
          icon: icons.dashboard,
          to: "/admin"
        }
      ];

      if (this.hasAnyPermission(DONATIONS_PERMISSIONS)) {
        items.push(
          {
            divider: true
          },
          {
            header: "components.sidebar.donations.title"
          },
          {
            title: "components.sidebar.donations.fountains.title",
            icon: icons.donations,
            to: "#",
            children: [
              {
                title: "components.sidebar.donations.fountains.all",
                to: "/admin/donations/fountains/list"
              },
              {
                title: "components.sidebar.donations.fountains.bks",
                to: "/admin/donations/fountains/list?projectCode=bks"
              },
              {
                title: "components.sidebar.donations.fountains.bgs",
                to: "/admin/donations/fountains/list?projectCode=bgs"
              },
              {
                title: "components.sidebar.donations.fountains.aki",
                to: "/admin/donations/fountains/list?projectCode=aki"
              },
              {
                title: "components.sidebar.donations.fountains.agi",
                to: "/admin/donations/fountains/list?projectCode=agi"
              },
              ...(this.hasPermission("Users.Create")
                ? [
                    {
                      title: "components.sidebar.donations.new",
                      to: "/admin/donations/fountains/new"
                    }
                  ]
                : [])
            ]
          }
        );
      }

      if (this.hasAnyPermission(TODOS_PERMISSIONS)) {
        items.push(
          { divider: true },
          { header: "components.sidebar.todos.header" },
          {
            title: "components.sidebar.todos.title",
            icon: icons.todos,
            to: "/admin/todos"
          }
        );
      }

      if (this.hasAnyPermission(ADMIN_PERMISSIONS)) {
        items.push({ divider: true }, { header: "components.sidebar.admin.title" });

        if (this.hasAnyPermission(USERS_PERMISSIONS)) {
          items.push({
            title: "components.sidebar.users.title",
            icon: icons.users,
            to: "#",
            children: [
              {
                title: "components.sidebar.users.list",
                to: "/admin/users/list"
              },
              ...(this.hasPermission("Users.Create")
                ? [
                    {
                      title: "components.sidebar.users.new",
                      to: "/admin/users/new"
                    }
                  ]
                : [])
            ]
          });
        }

        if (this.hasAnyPermission(ROLES_PERMISSIONS)) {
          items.push({
            title: "components.sidebar.roles.title",
            icon: icons.roles,
            to: "#",
            children: [
              {
                title: "components.sidebar.roles.list",
                to: "/admin/roles/list"
              },
              ...(this.hasPermission("Roles.Create")
                ? [
                    {
                      title: "components.sidebar.roles.new",
                      to: "/admin/roles/new"
                    }
                  ]
                : [])
            ]
          });
        }

        if (this.hasAnyPermission(TENANTS_PERMISSIONS)) {
          items.push({
            title: "components.sidebar.tenants.title",
            icon: icons.tenants,
            to: "#",
            children: [
              {
                title: "components.sidebar.tenants.list",
                to: "/admin/tenants/list"
              },
              ...(this.hasPermission("Tenants.Create")
                ? [
                    {
                      title: "components.sidebar.tenants.new",
                      to: "/admin/tenants/new"
                    }
                  ]
                : [])
            ]
          });
        }

        if (this.hasAnyPermission(AUDITS_PERMISSIONS)) {
          items.push({
            title: "components.sidebar.audits.title",
            icon: icons.audits,
            to: "/admin/audits"
          });
        }
      }

      this.items = items;
    }
  }
});
