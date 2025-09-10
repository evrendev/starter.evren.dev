import type { App } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import { routes } from "./routes";
import { useAuthStore } from "@/stores/auth";
import { useProfileStore } from "@/stores/profile";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const profileStore = useProfileStore();

  const requiresAuth = to.meta.requiresAuth;
  const requiredPermissions = to.meta.permissions as string[] | undefined;
  const isAuthenticated = authStore.isAuthenticated;

  if (isAuthenticated && to.name === "login") {
    const redirectPath = to.query.redirect as string | undefined;
    if (redirectPath) {
      return next(redirectPath);
    }
    return next({ name: "dashboard" });
  }

  if (!requiresAuth) {
    return next();
  }

  if (isAuthenticated) {
    if (profileStore.permissions.length === 0) {
      await profileStore.getPermissions();
    }

    if (
      requiredPermissions &&
      !profileStore.hasPermission(requiredPermissions)
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
