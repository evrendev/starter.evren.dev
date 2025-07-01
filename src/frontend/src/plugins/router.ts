import { createRouter, createWebHistory } from 'vue-router/auto'
import { routes } from 'vue-router/auto-routes'
import { setupLayouts } from 'virtual:meta-layouts'
import { useAuthStore } from '@/stores/auth'
import type { NavigationGuardNext, RouteLocationNormalized } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: setupLayouts(routes),
})

router.beforeResolve(
  async (to: RouteLocationNormalized, _: any, next: NavigationGuardNext) => {
    const authStore = useAuthStore()

    if (authStore.isLoading) {
      await authStore.getUserInfo()
      console.log('User info fetched')
    }

    if (
      to.meta.requiresPermission &&
      !authStore.hasPermission(to.meta.requiresPermission as string)
    ) {
      return next({ name: 'home' })
    } else if (to.meta.requiresAuth && !authStore.isAuthenticated) {
      return next({
        name: 'authentication-login',
        query: { redirect: to.fullPath },
      })
    } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
      return next({ name: 'home' })
    } else {
      return next()
    }
  },
)

export default router
