import { CircleIcon, KeyIcon, BugIcon, DashboardIcon } from "vue-tabler-icons"

const sidebarItem = [
  { header: "Dashboard" },
  {
    title: "Default",
    icon: DashboardIcon,
    to: "/dashboard",
  },
  { divider: true },
  { header: "Pages" },
  {
    title: "Authentication",
    icon: KeyIcon,
    to: "/auth",
    children: [
      {
        title: "Login",
        icon: CircleIcon,
        to: "/auth/login",
      },
    ],
  },
  {
    title: "Error 404",
    icon: BugIcon,
    to: "/error",
  },
]

export default sidebarItem
