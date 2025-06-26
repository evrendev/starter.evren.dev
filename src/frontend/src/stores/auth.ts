import { defineStore } from "pinia";
import http from "@/utils/http";
import router from "@/router";

interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  user: any | null; // UserDetailsDto veya UserBasicDto tipinde olabilir
}

export const useAuthStore = defineStore("auth", {
  state: (): AuthState => ({
    accessToken: localStorage.getItem("accessToken"),
    refreshToken: localStorage.getItem("refreshToken"),
    user: JSON.parse(localStorage.getItem("user") || "null"),
  }),
  getters: {
    isAuthenticated: (state) => !!state.accessToken,
    getUser: (state) => state.user,
  },
  actions: {
    async login(email: string, password: string, tenantId: string) {
      try {
        const response = await http.post(
          "/auth/login",
          { email, password },
          {
            headers: {
              Tenant: tenantId,
            },
          }
        );
        const { data } = response.data;
        this.accessToken = data.token;
        this.refreshToken = data.refreshToken;
        this.user = data.user;

        localStorage.setItem("accessToken", data.token);
        localStorage.setItem("refreshToken", data.refreshToken);
        localStorage.setItem("user", JSON.stringify(data.user));
        localStorage.setItem("tenantId", tenantId);
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },
    async refreshToken() {
      try {
        const response = await http.get("/auth/refresh-token");
        const { data } = response.data;
        this.accessToken = data.token;
        this.refreshToken = data.refreshToken;
        this.user = data.user;

        localStorage.setItem("accessToken", data.token);
        localStorage.setItem("refreshToken", data.refreshToken);
        localStorage.setItem("user", JSON.stringify(data.user));
        return true;
      } catch (error) {
        console.error("Failed to refresh token:", error);
        this.logout();
        throw error;
      }
    },
    async logout() {
      try {
        const currentTenantId =
          localStorage.getItem("tenantId") ||
          (import.meta.env.VITE_APP_DEFAULT_TENANT_ID as string);
        await http.post(
          "/auth/logout",
          {},
          {
            headers: {
              Tenant: currentTenantId,
            },
          }
        );
      } catch (error) {
        console.error(
          "Logout API call failed, but clearing local data:",
          error
        );
      } finally {
        this.accessToken = null;
        this.refreshToken = null;
        this.user = null;
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");
        localStorage.removeItem("tenantId");
        router.push({ name: "login" });
      }
    },
    setLanguage(lang: string) {
      localStorage.setItem("language", lang);
      // Vue-i18n instance'ının locale'ini güncellemek için
      // i18n global instance'ına erişiminiz varsa: i18n.global.locale.value = lang;
      // Şu anki setup'ımızda, plugins/i18n.ts'de export edilen i18n objesine direkt erişim yok
      // En basit yol, uygulamayı yeniden yüklemek veya i18n'i global olarak güncelleyebilecek bir olay fırlatmak.
      // Ya da i18n instance'ını Pinia store'a inject edebilirsiniz.
    },
    setTenantId(tenantId: string) {
      localStorage.setItem("tenantId", tenantId);
      // Genellikle tenant değişimi sonrası yeniden oturum açma veya API client'ı yeniden başlatma gerekir.
    },
  },
});
