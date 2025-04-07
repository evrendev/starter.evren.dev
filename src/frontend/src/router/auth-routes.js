const AuthRoutes = {
  path: "/auth",
  component: () => import("@/layouts/blank/BlankLayout.vue"),
  children: [
    {
      path: ":page?",
      name: "login",
      component: () => import("@/views/authentication/IndexPage.vue"),
      props: (route) => ({
        page: route.params.page || "login"
      }),
      meta: {
        titleKey: "auth.login.title"
      }
    }
  ]
};

export default AuthRoutes;
