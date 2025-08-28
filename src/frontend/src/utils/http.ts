import axios, {
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";

const API_BASE_URL = import.meta.env.VITE_APP_API_BASE_URL as string;
const DEFAULT_TENANT_ID = import.meta.env.VITE_APP_DEFAULT_TENANT_ID as string;
const DEFAULT_LANGUAGE = import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string;

const http: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

http.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const currentTenantId =
      localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;
    const currentLanguage =
      localStorage.getItem("language") || DEFAULT_LANGUAGE;

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
  },
);

export default http;
