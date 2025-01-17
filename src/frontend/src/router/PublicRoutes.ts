const PublicRoutes = {
  path: '/',
  redirect: '/dashboard/home',
  children: [
    {
      name: 'Authentication',
      path: '/auth/login',
      component: () => import('@/views/authentication/LoginPage.vue')
    },
    {
      name: 'Error 404',
      path: '/error',
      component: () => import('@/views/pages/maintenance/error/Error404Page.vue')
    }
  ]
};

export default PublicRoutes;
