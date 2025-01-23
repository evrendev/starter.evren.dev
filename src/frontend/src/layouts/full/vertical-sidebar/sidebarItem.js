import { DashboardIcon, ListCheckIcon, UsersIcon, Category2Icon, GitCompareIcon } from "vue-tabler-icons"; // Permission Groups
import { useAuthStore } from "@/stores/auth";

const TODOS_PERMISSIONS = ["Todos.Read", "Todos.Create", "Todos.Edit", "Todos.Delete"];
const USERS_PERMISSIONS = ["Users.Read", "Users.Create", "Users.Edit", "Users.Delete"];
const ROLES_PERMISSIONS = ["Roles.Read", "Roles.Create", "Roles.Edit", "Roles.Delete"];
const TENANTS_PERMISSIONS = ["Tenants.Read", "Tenants.Create", "Tenants.Edit", "Tenants.Delete"];
const ADMIN_PERMISSIONS = [...USERS_PERMISSIONS, ...ROLES_PERMISSIONS, ...TENANTS_PERMISSIONS];

// Permission Helpers
const hasPermission = (permission) => {
  const authStore = useAuthStore();
  return authStore.user?.permissions?.includes(permission);
};

const hasAnyPermission = (permissions) => {
  return permissions.some((permission) => hasPermission(permission));
};

const sidebarItem = [
  { header: "components.sidebar.dashboard.header" },
  {
    title: "components.sidebar.dashboard.title",
    icon: DashboardIcon,
    to: "/dashboard"
  },
  ...(hasAnyPermission(TODOS_PERMISSIONS)
    ? [
        { divider: true },
        { header: "components.sidebar.todos.header" },
        {
          title: "components.sidebar.todos.title",
          icon: ListCheckIcon,
          to: "/dashboard/todos"
        }
      ]
    : []),
  ...(hasAnyPermission(ADMIN_PERMISSIONS)
    ? [
        { divider: true },
        { header: "components.sidebar.admin.title" },
        ...(hasAnyPermission(USERS_PERMISSIONS)
          ? [
              {
                title: "components.sidebar.users.title",
                icon: UsersIcon,
                to: "/dashboard/users",
                children: [
                  {
                    title: "components.sidebar.users.list",
                    to: "/dashboard/users"
                  },
                  ...(hasPermission("Users.Create")
                    ? [
                        {
                          title: "components.sidebar.users.new",
                          to: "/dashboard/users/new"
                        }
                      ]
                    : [])
                ]
              }
            ]
          : []),
        ...(hasAnyPermission(ROLES_PERMISSIONS)
          ? [
              {
                title: "components.sidebar.roles.title",
                icon: Category2Icon,
                to: "/dashboard/roles",
                children: [
                  {
                    title: "components.sidebar.roles.list",
                    to: "/dashboard/roles"
                  },
                  ...(hasPermission("Roles.Create")
                    ? [
                        {
                          title: "components.sidebar.roles.new",
                          to: "/dashboard/roles/new"
                        }
                      ]
                    : [])
                ]
              }
            ]
          : []),
        ...(hasAnyPermission(TENANTS_PERMISSIONS)
          ? [
              {
                title: "components.sidebar.tenants.title",
                icon: GitCompareIcon,
                to: "/dashboard/tenants",
                children: [
                  {
                    title: "components.sidebar.tenants.list",
                    to: "/dashboard/tenants"
                  },
                  ...(hasPermission("Roles.Create")
                    ? [
                        {
                          title: "components.sidebar.tenants.new",
                          to: "/dashboard/tenants/new"
                        }
                      ]
                    : [])
                ]
              }
            ]
          : [])
      ]
    : [])
];

export default sidebarItem;
