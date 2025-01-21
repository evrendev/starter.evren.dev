import { defineStore } from "pinia";
import { router } from "@/router";
import axiosInstance from "@/plugins/axios";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuthStore = defineStore({
  id: "auth",
  state: () => ({
    user: null,
    userId: null,
    token: null,
    refreshToken: null,
    returnUrl: null,
    rememberMe: false
  }),
  actions: {
    async login(email, password, rememberMe, response) {
      const user = await axiosInstance.post(`${baseUrl}/auth/login`, {
        email,
        password,
        rememberMe,
        response
      });

      if (user?.data?.requiresTwoFactor) {
        this.userId = user?.data?.userId;
        this.rememberMe = rememberMe;

        router.push(`/auth/2fa`);
      } else {
        // update pinia state
        this.user = user?.data.user;
        localStorage.setItem("user", JSON.stringify(this.user));

        this.token = user?.data.token;
        localStorage.setItem("token", this.token);

        this.refreshToken = user?.data.refreshToken;
        localStorage.setItem("refreshToken", this.refreshToken);

        router.push(this.returnUrl || "/dashboard");
      }
    },
    async verify(code) {
      const rememberMachine = this.rememberMe;
      const cleanCode = code.replace(/\s+/g, "").replace(/[^0-9]/g, "");

      const userId = this.userId;
      if (!userId) router.push("/auth/login");

      const user = await axiosInstance.post(`${baseUrl}/2fa/verify`, {
        userId: userId,
        code: cleanCode,
        rememberMachine: rememberMachine
      });

      // update pinia state
      this.user = user?.data.user;
      this.token = user?.data.token;
      this.refreshToken = user?.data.refreshToken;

      router.push(this.returnUrl || "/dashboard");
    },
    logout() {
      this.user = null;
      this.token = null;
      this.refreshToken = null;
      this.userId = null;
      this.rememberMe = false;

      localStorage.removeItem("user");
      router.push("/auth/login");
    },
    setReturnUrl(url) {
      this.returnUrl = url;
    }
  }
});
