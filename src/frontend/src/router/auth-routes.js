const AuthRoutes = {
  name: "login",
  path: "/auth/:page?",
  component: () => import("@/views/authentication/IndexPage.vue"),
  props: (route) => ({
    page: route.params.page || "login"
  }),
  meta: {
    titleKey: "auth.login.title"
  }
};

export default AuthRoutes;
