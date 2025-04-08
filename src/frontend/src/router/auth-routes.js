const AuthRoutes = {
  path: "/auth",
  component: () => import("@/layouts/auth/AuthLayout.vue"),
  children: [
    {
      path: "",
      alias: ["login", "sign-in"],
      name: "login",
      component: () => import("@/views/authentication/LogIn.vue"),
      meta: {
        titleKey: "auth.login.title"
      }
    },
    {
      path: "forgot-password",
      name: "forgot-password",
      component: () => import("@/views/authentication/ForgotPassword.vue"),
      meta: {
        titleKey: "auth.forgotPassword.title"
      }
    },
    {
      path: "2fa",
      name: "2fa",
      component: () => import("@/views/authentication/TwoFactorAuthentication.vue"),
      meta: {
        titleKey: "auth.2fa.title"
      }
    }
  ]
};

export default AuthRoutes;
