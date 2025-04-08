import { defineStore } from "pinia";
import { getActivePinia } from "pinia";
import { router } from "@/router";
import { useLocalStorage } from "@vueuse/core";
import { apiService } from "@/utils/helpers";
import LocaleHelper from "@/utils/helpers/locale";

export const useAuthStore = defineStore("auth", {
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
            id: res?.userId
          }
        };

        router.push({ name: "2fa" });
      } else {
        this.auth = {
          isAuthenticated: true,
          user: res?.user,
          token: res?.token,
          refreshToken: res?.refreshToken
        };

        await LocaleHelper.switchLanguage(res?.user.language);
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
    async verify(data) {
      const res = await apiService.post("/2fa/verify", data);

      this.auth = {
        isAuthenticated: true,
        user: res?.user,
        token: res?.token,
        refreshToken: res?.refreshToken
      };

      router.push(this.returnUrl || "/admin");
    },
    async logout() {
      await apiService.post("/auth/logout");
      getActivePinia()._s.forEach((store) => store.$reset());
      router.push({ name: "login", query: { returnUrl: router.currentRoute.value.fullPath } });
    },
    setReturnUrl(url) {
      this.auth = {
        returnUrl: url
      };
    },
    updateUser(userData) {
      this.auth = {
        ...this.auth,
        user: {
          ...this.auth.user,
          ...userData
        }
      };
    }
  }
});
