import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useAppStore } from "@/stores/app";
import PanelRoutes from "./panel-routes";
import AuthRoutes from "./auth-routes";
import { storeToRefs } from "pinia";

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  linkActiveClass: "active",
  linkExactActiveClass: "exact-active",
  routes: [
    {
      path: "",
      redirect: "/dashboard"
    },
    {
      path: "/:pathMatch(.*)*",
      component: () => import("@/views/pages/maintenance/error/Error404Page.vue")
    },
    PanelRoutes,
    AuthRoutes
  ]
});

router.beforeEach(async (to, from, next) => {
  const publicPages = ["/"];
  const authStore = useAuthStore();
  const { user, returnUrl } = storeToRefs(authStore);

  const isPublicPage = publicPages.includes(to.path);
  const authRequired = !isPublicPage && to.matched.some((record) => record.meta.requiresAuth);

  if (authRequired && !user) {
    authStore.setReturnUrl(to.fullPath);
    next("/auth/login");
  } else if (user && to.path === "/auth/login") {
    next({
      query: {
        ...to.query,
        redirect: returnUrl !== "/" ? to.fullPath : undefined
      }
    });
  } else {
    next();
  }
});

router.afterEach(() => {
  const app = useAppStore();
  app.setPageLoader(false);
});
