import { createRouter, createWebHistory } from 'vue-router/auto'
import { routes } from 'vue-router/auto-routes'
import { setupLayouts } from 'virtual:meta-layouts'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: setupLayouts(routes),
})

router.beforeResolve(async (to, _: any, next) => {
  const authStore = useAuthStore()

  if (authStore.isLoading) {
    await authStore.getUserInfo()
    console.log('User info fetched')
  }

  if (
    to.meta.requiresPermission &&
    !authStore.hasPermission(to.meta.requiresPermission as string)
  ) {
    return next('/home')
  } else if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/auth/login?redirect=' + encodeURIComponent(to.fullPath))
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return next('/home')
  } else {
    return next()
  }
})

export default router
