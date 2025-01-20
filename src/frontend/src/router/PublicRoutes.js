const PublicRoutes = {
  path: "/",
  redirect: "/dashboard/home",
  children: [
    {
      name: "Login",
      path: "/auth/login",
      component: () => import("@/views/authentication/LoginPage.vue")
    },
    {
      name: "2FA",
      path: "/auth/login/2fa",
      component: () => import("@/views/authentication/TwoFactorAuthenticationPage.vue")
    },
    {
      name: "ForgotPassword",
      path: "/auth/login/forgot-password",
      component: () => import("@/views/authentication/ForgotPasswordPage.vue")
    },
    {
      name: "Error 404",
      path: "/error",
      component: () => import("@/views/pages/maintenance/error/Error404Page.vue")
    }
  ]
};

export default PublicRoutes;
