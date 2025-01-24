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
      returnUrl: null,
      rememberMe: false
    })
  }),
  getters: {
    isAuthenticated: (state) => state.auth.isAuthenticated,
    user: (state) => state.auth.user,
    token: (state) => state.auth.token,
    refreshToken: (state) => state.auth.refreshToken,
    returnUrl: (state) => state.auth.returnUrl
  },
  actions: {
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

        router.push(this.returnUrl || "/admin");
      }
    },
    async refresh() {
      const res = await apiService.post("/auth/refresh-token", {
        userId: this.user?.id,
        refreshToken: this.refreshToken
      });

      this.auth = {
        isAuthenticated: true,
        user: res?.user,
        token: res?.token,
        refreshToken: res?.refreshToken
      };
    },
    logout() {
      this.auth = null;
      router.push("/auth/login");
    },
    setReturnUrl(url) {
      this.auth = {
        returnUrl: url
      };
    }
  }
});
