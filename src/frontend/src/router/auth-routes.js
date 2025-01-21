const AuthRoutes = {
  name: "login",
  path: "/auth/:page?",
  component: () => import("@/views/authentication/IndexPage.vue"),
  props: (route) => ({
    page: route.params.page || "login"
  })
};

export default AuthRoutes;
