import type { App } from "vue";
import {
  createRouter,
  createWebHistory,
  type RouteLocationNormalized,
} from "vue-router";
import { routes } from "./routes";
import { useAuthStore } from "@/stores/auth";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeResolve(async (to, from: RouteLocationNormalized, next) => {
  const authStore = useAuthStore();

  if (authStore.isLoading) {
    await authStore.getUserInfo();
  }

  if (
    to.meta.requiresPermission &&
    !authStore.hasPermission(to.meta.requiresPermission as string)
  ) {
    return next({ name: "unauthorized" });
  } else if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next({ name: "login", query: { redirect: to.fullPath } });
  } else if (to.name === "login" && authStore.isAuthenticated) {
    return next({ name: "dashboard" });
  } else {
    return next();
  }
});

export default function (app: App) {
  app.use(router);
}

export { router };
