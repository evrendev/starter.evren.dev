import type { App } from "vue";
import { createRouter, createWebHistory } from "vue-router";
import { routes } from "./routes";
import { useAuthStore } from "@/stores/auth";
import { usePersonalStore } from "@/stores/personal";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  const personalStore = usePersonalStore();

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
