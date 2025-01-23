import { DashboardIcon, ListCheckIcon, UsersIcon, Category2Icon, GitCompareIcon } from "vue-tabler-icons";

const sidebarItem = [
  { header: "components.sidebar.dashboard.header" },
  {
    title: "components.sidebar.dashboard.title",
    icon: DashboardIcon,
    to: "/dashboard"
  },
  { divider: true },
  { header: "components.sidebar.todos.header" },
  {
    title: "components.sidebar.todos.title",
    icon: ListCheckIcon,
    to: "/dashboard/todos"
  },
  { divider: true },
  { header: "components.sidebar.admin.title" },
  {
    title: "components.sidebar.users.title",
    icon: UsersIcon,
    to: "/dashboard/users",
    children: [
      {
        title: "components.sidebar.users.list",
        to: "/dashboard/users"
      },
      {
        title: "components.sidebar.users.new",
        to: "/dashboard/users/new"
      }
    ]
  },
  {
    title: "components.sidebar.roles.title",
    icon: Category2Icon,
    to: "/dashboard/roles",
    children: [
      {
        title: "components.sidebar.roles.list",
        to: "/dashboard/roles"
      },
      {
        title: "components.sidebar.roles.new",
        to: "/dashboard/roles/new"
      }
    ]
  },
  {
    title: "components.sidebar.tenants.title",
    icon: GitCompareIcon,
    to: "/dashboard/tenants",
    children: [
      {
        title: "components.sidebar.tenants.list",
        to: "/dashboard/tenants"
      },
      {
        title: "components.sidebar.tenants.new",
        to: "/dashboard/tenants/new"
      }
    ]
  }
];

export default sidebarItem;
