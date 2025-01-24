import { createRouter, createWebHistory } from "vue-router";
import { useAuthStore, useAppStore } from "@/stores";
import AdminRoutes from "./admin-routes";
import AuthRoutes from "./auth-routes";

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  linkActiveClass: "active",
  linkExactActiveClass: "exact-active",
  routes: [
    {
      path: "",
      redirect: "/admin"
    },
    {
      path: "/:pathMatch(.*)*",
      component: () => import("@/views/errors/Error404Page.vue")
    },
    AdminRoutes,
    AuthRoutes
  ]
});

router.beforeEach(async (to, from, next) => {
  const publicPages = ["/"];
  const authStore = useAuthStore();
  const { user, returnUrl } = authStore;

  const isPublicPage = publicPages.includes(to.path);
  const authRequired = !isPublicPage && to.matched.some((record) => record.meta.requiresAuth);

  // console.log(isPublicPage, authRequired, user.value, to.fullPath);

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
  const appStore = useAppStore();
  appStore.setPageLoader(false);
});
