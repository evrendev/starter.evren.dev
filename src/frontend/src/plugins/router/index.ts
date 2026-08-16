import type { App } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import { routes } from "./routes";
import { useAuthStore } from "@/stores/auth";
import { usePersonalStore } from "@/stores/personal";
import { Permissions } from "@/models/user";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  linkActiveClass: "active",
  linkExactActiveClass: "exact-active",
  routes,
});

// Accounts without admin-panel access (e.g. Student, via self-register —
// see Task O2) land on the learner-facing catalog instead of /admin/dashboard,
// which they have no permission to view.
export async function getPostLoginRoute(): Promise<{ name: string }> {
  const personalStore = usePersonalStore();
  if (personalStore.permissions.length === 0) {
    await personalStore.getPermissions();
  }

  return personalStore.hasPermission([Permissions.DashboardView])
    ? { name: "dashboard" }
    : { name: "learning-my-courses" };
}

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const personalStore = usePersonalStore();

  const requiresAuth = to.meta.requiresAuth;
  const requiredPermissions = to.meta.requiresPermission as
    | string[]
    | undefined;
  const isAuthenticated = authStore.isAuthenticated;

  if (isAuthenticated && to.name === "login") {
    const redirectPath = to.query.redirect as string | undefined;
    if (redirectPath) {
      return next(redirectPath);
    }
    return next(await getPostLoginRoute());
  }

  if (!requiresAuth) {
    return next();
  }

  if (isAuthenticated) {
    if (personalStore.permissions.length === 0) {
      await personalStore.getPermissions();
    }

    if (
      requiredPermissions &&
      !personalStore.hasPermission(requiredPermissions)
    ) {
      return next({ name: "unauthorized" });
    }

    return next();
  } else {
    return next({ name: "login", query: { redirect: to.fullPath } });
  }
});

export default function (app: App) {
  app.use(router);
}

export { router };
