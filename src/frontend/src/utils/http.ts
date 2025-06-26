// src/utils/http.ts
import axios, {
  type AxiosInstance,
  type AxiosRequestConfig,
  type AxiosResponse,
} from "axios";
import { useAuthStore } from "@/stores/auth";

// Bu değişkenin değeri .env dosyasından doğru bir şekilde okunmalı
const API_BASE_URL = import.meta.env.VITE_APP_API_BASE_URL as string;
const DEFAULT_TENANT_ID = import.meta.env.VITE_APP_DEFAULT_TENANT_ID as string;

const http: AxiosInstance = axios.create({
  baseURL: API_BASE_URL, // Burası çok önemli!
  headers: {
    "Content-Type": "application/json",
  },
});

http.interceptors.request.use(
  (config: AxiosRequestConfig) => {
    // Burada type annotation kullanmaya devam edin
    const authStore = useAuthStore();
    const token = authStore.accessToken;
    const currentTenantId =
      localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;
    const currentLanguage = localStorage.getItem("language") || "en";

    if (token) {
      config.headers = config.headers || {};
      config.headers["Authorization"] = `Bearer ${token}`;
    }

    if (currentTenantId) {
      config.headers = config.headers || {};
      config.headers["Tenant"] = currentTenantId;
    }

    if (currentLanguage) {
      config.headers = config.headers || {};
      config.headers["Accept-Language"] = currentLanguage;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

http.interceptors.response.use(
  (response: AxiosResponse) => {
    // Burada da type annotation kullanmaya devam edin
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      const authStore = useAuthStore();
      try {
        await authStore.refreshToken();
        const newToken = authStore.accessToken;
        if (newToken) {
          originalRequest.headers = originalRequest.headers || {}; // headers'ın undefined olmaması için kontrol
          originalRequest.headers["Authorization"] = `Bearer ${newToken}`;
          return http(originalRequest);
        }
      } catch (refreshError) {
        console.error("Refresh token failed:", refreshError);
        authStore.logout();
        // Pencerenin mevcut location'ından /login'e yönlendir
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export default http;
