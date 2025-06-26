// src/router/index.ts

// Eski hali:
// import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';

// Yeni hali: RouteRecordRaw tipini 'import type' olarak import edin
import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from "vue-router";

import LoginView from "@/views/auth/LoginView.vue";
import DashboardView from "@/views/DashboardView.vue";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "home",
    redirect: "/dashboard",
  },
  {
    path: "/dashboard",
    name: "dashboard",
    component: DashboardView,
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: "/login",
    name: "login",
    component: LoginView,
    meta: {
      public: true,
    },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem("accessToken");
  if (to.meta.requiresAuth && !isAuthenticated) {
    next({ name: "login" });
  } else if (to.meta.public && isAuthenticated) {
    next({ name: "dashboard" });
  } else {
    next();
  }
});

export default router;
