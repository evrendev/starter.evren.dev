import { defineStore } from "pinia";
import { router } from "@/router";
import { useLocalStorage } from "@vueuse/core";
import { apiService } from "@/utils/helpers";

export const useAuthStore = defineStore({
  id: "auth",
  state: () => ({
    auth: useLocalStorage("auth", {
      isAuthenticated: false,
      user: null,
      token: null,
      refreshToken: null,
      returnUrl: null
    })
  }),
  getters: {
    isAuthenticated: (state) => state.auth.isAuthenticated,
    user: (state) => state.auth.user,
    token: (state) => state.auth.token,
    refreshToken: (state) => state.auth.refreshToken,
    returnUrl: (state) => state.auth.returnUrl ?? "/dashboard"
  },
  actions: {
    setReturnUrl(url) {
      this.auth = {
        returnUrl: url
      };
    },
    async login(data) {
      const res = await apiService.post("/auth/login", data);

      if (res?.requiresTwoFactor) {
        this.auth = {
          user: {
            id: res?.user?.id
          }
        };

        router.push(`/auth/2fa`);
      } else {
        this.auth = {
          isAuthenticated: true,
          user: res?.user,
          token: res?.token,
          refreshToken: res?.refreshToken
        };

        router.push(this.returnUrl || "/dashboard");
        window.location.reload();
      }
    },
    // async verify(code) {
    //   const rememberMachine = this.rememberMe;
    //   const cleanCode = code.replace(/\s+/g, "").replace(/[^0-9]/g, "");

    //   const userId = this.userId;
    //   if (!userId) router.push("/auth/login");

    //   const user = await axiosInstance.post(`${baseUrl}/2fa/verify`, {
    //     userId: userId,
    //     code: cleanCode,
    //     rememberMachine: rememberMachine
    //   });

    //   // update pinia state
    //   this.user = user?.data.user;
    //   this.token = user?.data.token;
    //   this.refreshToken = user?.data.refreshToken;

    //   router.push(this.returnUrl || "/dashboard");
    // },
    logout() {
      this.auth = {};

      localStorage.removeItem("user");
      router.push("/auth/login");
    }
  }
});
