import { Permissions } from "@/models/user";

export const adminRoutes = [
  {
    path: "/admin",
    component: () => import("@/layouts/admin.vue"),
    meta: {
      requiresAuth: true,
    },
    children: [
      {
        name: "dashboard",
        path: "",
        component: () => import("@/pages/admin/dashboard.vue"),
        meta: {
          requiresPermission: [Permissions.DashboardView],
        },
      },
      {
        name: "tenants",
        path: "tenants",
        children: [
          {
            name: "tenant-list",
            path: "",
            component: () => import("@/pages/admin/tenants/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.TenantView,
                Permissions.TenantSearch,
              ],
              title: "admin.tenants.list.title",
            },
          },
          {
            name: "tenant-create",
            path: "create",
            component: () => import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: [Permissions.TenantCreate],
              title: "admin.tenants.create.title",
            },
          },
          {
            name: "tenant-view",
            path: ":id/view",
            component: () => import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: [Permissions.TenantView],
              title: "admin.tenants.view.title",
            },
          },
          {
            name: "tenant-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/tenants/form.vue"),
            meta: {
              requiresPermission: [Permissions.TenantUpdate],
              title: "admin.tenants.edit.title",
            },
          },
        ],
      },
      {
        name: "roles",
        path: "roles",
        children: [
          {
            name: "role-list",
            path: "",
            component: () => import("@/pages/admin/roles/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.RoleView,
                Permissions.RoleSearch,
              ],
              title: "admin.roles.list.title",
            },
          },
          {
            name: "role-create",
            path: "create",
            component: () => import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: [Permissions.RoleCreate],
              title: "admin.roles.create.title",
            },
          },
          {
            name: "role-view",
            path: ":id/view",
            component: () => import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: [Permissions.RoleView],
              title: "admin.roles.view.title",
            },
          },
          {
            name: "role-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/roles/form.vue"),
            meta: {
              requiresPermission: [Permissions.RoleUpdate],
              title: "admin.roles.edit.title",
            },
          },
        ],
      },
      {
        name: "personal",
        path: "personal",
        redirect: { name: "personal-profile" },
        children: [
          {
            name: "personal-profile",
            path: "profile",
            component: () => import("@/pages/admin/personal/profile.vue"),
            meta: {
              title: "admin.personal.profile.title",
            },
          },
          {
            name: "personal-security",
            path: "security",
            component: () => import("@/pages/admin/personal/security.vue"),
            meta: {
              title: "admin.personal.security.title",
            },
          },
          {
            name: "personal-logs",
            path: "logs",
            component: () => import("@/pages/admin/personal/logs.vue"),
            meta: {
              title: "admin.personal.logs.title",
            },
          },
        ],
      },
      {
        name: "users",
        path: "users",
        children: [
          {
            name: "user-list",
            path: "",
            component: () => import("@/pages/admin/users/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.UserView,
                Permissions.UserSearch,
              ],
              title: "admin.users.list.title",
            },
          },
          {
            name: "user-create",
            path: "create",
            component: () => import("@/pages/admin/users/form.vue"),
            meta: {
              requiresPermission: [Permissions.UserCreate],
              title: "admin.users.create.title",
            },
          },
          {
            name: "user-view",
            path: ":id/view",
            component: () => import("@/pages/admin/users/form.vue"),
            meta: {
              requiresPermission: [Permissions.UserView],
              title: "admin.users.view.title",
            },
          },
          {
            name: "user-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/users/form.vue"),
            meta: {
              requiresPermission: [Permissions.UserUpdate],
              title: "admin.users.edit.title",
            },
          },
        ],
      },
      {
        name: "categories",
        path: "categories",
        children: [
          {
            name: "category-list",
            path: "",
            component: () => import("@/pages/admin/categories/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.CategoryView,
                Permissions.CategorySearch,
              ],
              title: "admin.categories.list.title",
            },
          },
          {
            name: "category-create",
            path: "create",
            component: () => import("@/pages/admin/categories/form.vue"),
            meta: {
              requiresPermission: [Permissions.CategoryCreate],
              title: "admin.categories.create.title",
            },
          },
          {
            name: "category-view",
            path: ":id/view",
            component: () => import("@/pages/admin/categories/form.vue"),
            meta: {
              requiresPermission: [Permissions.CategoryView],
              title: "admin.categories.view.title",
            },
          },
          {
            name: "category-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/categories/form.vue"),
            meta: {
              requiresPermission: [Permissions.CategoryUpdate],
              title: "admin.categories.edit.title",
            },
          },
        ],
      },
      {
        name: "courses",
        path: "courses",
        children: [
          {
            name: "course-list",
            path: "",
            component: () => import("@/pages/admin/courses/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.CourseView,
                Permissions.CourseSearch,
              ],
              title: "admin.courses.list.title",
            },
          },
          {
            name: "course-create",
            path: "create",
            component: () => import("@/pages/admin/courses/form.vue"),
            meta: {
              requiresPermission: [Permissions.CourseCreate],
              title: "admin.courses.create.title",
            },
          },
          {
            name: "course-view",
            path: ":id/view",
            component: () => import("@/pages/admin/courses/form.vue"),
            meta: {
              requiresPermission: [Permissions.CourseView],
              title: "admin.courses.view.title",
            },
          },
          {
            name: "course-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/courses/form.vue"),
            meta: {
              requiresPermission: [Permissions.CourseUpdate],
              title: "admin.courses.edit.title",
            },
          },
        ],
      },
      {
        name: "chapters",
        path: "chapters",
        children: [
          {
            name: "chapter-list",
            path: "",
            component: () => import("@/pages/admin/chapters/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.ChapterView,
                Permissions.ChapterSearch,
              ],
              title: "admin.chapters.list.title",
            },
          },
          {
            name: "chapter-create",
            path: "create",
            component: () => import("@/pages/admin/chapters/form.vue"),
            meta: {
              requiresPermission: [Permissions.ChapterCreate],
              title: "admin.chapters.create.title",
            },
          },
          {
            name: "chapter-view",
            path: ":id/view",
            component: () => import("@/pages/admin/chapters/form.vue"),
            meta: {
              requiresPermission: [Permissions.ChapterView],
              title: "admin.chapters.view.title",
            },
          },
          {
            name: "chapter-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/chapters/form.vue"),
            meta: {
              requiresPermission: [Permissions.ChapterUpdate],
              title: "admin.chapters.edit.title",
            },
          },
        ],
      },
      {
        name: "lessons",
        path: "lessons",
        children: [
          {
            name: "lesson-list",
            path: "",
            component: () => import("@/pages/admin/lessons/index.vue"),
            meta: {
              requiresPermission: [
                Permissions.LessonView,
                Permissions.LessonSearch,
              ],
              title: "admin.lessons.list.title",
            },
          },
          {
            name: "lesson-create",
            path: "create",
            component: () => import("@/pages/admin/lessons/form.vue"),
            meta: {
              requiresPermission: [Permissions.LessonCreate],
              title: "admin.lessons.create.title",
            },
          },
          {
            name: "lesson-view",
            path: ":id/view",
            component: () => import("@/pages/admin/lessons/form.vue"),
            meta: {
              requiresPermission: [Permissions.LessonView],
              title: "admin.lessons.view.title",
            },
          },
          {
            name: "lesson-edit",
            path: ":id/edit",
            component: () => import("@/pages/admin/lessons/form.vue"),
            meta: {
              requiresPermission: [Permissions.LessonUpdate],
              title: "admin.lessons.edit.title",
            },
          },
        ],
      },
      {
        name: "unauthorized",
        path: "unauthorized",
        component: () => import("@/pages/admin/unauthorized.vue"),
      },
    ],
  },
];
