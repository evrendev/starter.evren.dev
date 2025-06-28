import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/auth',
      name: 'auth',
      children: [
        {
          path: 'login',
          name: 'auth-login',
          component: () => import('@/views/Authentication/LoginView.vue')
        },
        {
          path: 'permissions',
          name: 'auth-permissions',
          component: () => import('@/views/Authentication/PermissionsView.vue'),
          meta: { requiresAuth: true }
        }
      ]
    }
  ]
})

router.beforeResolve(async (to, _, next) => {
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
    return next({ name: 'authentication-login', query: { redirect: to.fullPath } })
  } else if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return next({ name: 'home' })
  } else {
    return next()
  }
})

export default router
