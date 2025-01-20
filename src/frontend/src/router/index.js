import { createRouter, createWebHistory } from "vue-router";
import MainRoutes from "./MainRoutes";
import PublicRoutes from "./PublicRoutes";
import { useAuthStore } from "@/stores/auth";

export const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/:pathMatch(.*)*",
      component: () => import("@/views/pages/maintenance/error/Error404Page.vue")
    },
    MainRoutes,
    PublicRoutes
  ]
});

router.beforeEach(async (to, from, next) => {
  // redirect to login page if not logged in and trying to access a restricted page
  const publicPages = ["/"];
  const auth = useAuthStore();

  const isPublicPage = publicPages.includes(to.path);
  const authRequired = !isPublicPage && to.matched.some((record) => record.meta.requiresAuth);

  // User not logged in and trying to access a restricted page
  if (authRequired && !auth.user) {
    auth.returnUrl = to.fullPath; // Save the intended page
    next("/auth/login");
  } else if (auth.user && to.path === "/auth/login") {
    // User logged in and trying to access the login page
    next({
      query: {
        ...to.query,
        redirect: auth.returnUrl !== "/" ? to.fullPath : undefined
      }
    });
  } else {
    // All other scenarios, either public page or authorized access
    next();
  }
});
