const MainRoutes = {
  path: '/main',
  meta: {
    requiresAuth: true
  },
  redirect: '/main/dashboard/default',
  component: () => import('@/layouts/full/FullLayout.vue'),
  children: [
    {
      name: 'StarterPage',
      path: '/',
      component: () => import('@/views/StarterPage.vue')
    }
  ]
};

export default MainRoutes;
