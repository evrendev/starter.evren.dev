import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";
import { Result } from "@/primitives/result";
import { AppError } from "@/primitives/error";
import type { ApiErrorResponse } from "@/types/responses/api";
import { useAuthStore } from "@/stores/auth";

const API_BASE_URL = import.meta.env.VITE_APP_API_BASE_URL as string;
const http: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

http.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore();

    const DEFAULT_TENANT_ID = import.meta.env
      .VITE_APP_DEFAULT_TENANT_ID as string;
    const DEFAULT_LANGUAGE = import.meta.env
      .VITE_APP_DEFAULT_LANGUAGE as string;
    const currentTenantId =
      localStorage.getItem("tenantId") || DEFAULT_TENANT_ID;
    const currentLanguage =
      localStorage.getItem("language") || DEFAULT_LANGUAGE;

    if (currentTenantId) config.headers["Tenant"] = currentTenantId;
    if (currentLanguage) config.headers["Accept-Language"] = currentLanguage;

    if (authStore.accessToken && !config.headers["Authorization"]) {
      config.headers["Authorization"] = `Bearer ${authStore.accessToken}`;
    }

    return config;
  },
  (error) => Promise.reject(error),
);

let isRefreshing = false;
let failedQueue: {
  resolve: (value: unknown) => void;
  reject: (reason?: any) => void;
}[] = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

http.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };
    const authStore = useAuthStore();

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      authStore.refreshToken
    ) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers!["Authorization"] = "Bearer " + token;
            return http(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const result = await authStore.refresh();
        if (result.succeeded) {
          processQueue(null, authStore.accessToken);
          originalRequest.headers!["Authorization"] =
            `Bearer ${authStore.accessToken}`;
          return http(originalRequest);
        } else {
          processQueue(result.errors, null);
          await authStore.logout();
          return Promise.reject(Result.failure(result.errors!));
        }
      } catch (refreshError) {
        processQueue(refreshError, null);
        await authStore.logout();
        return Promise.reject(
          Result.failure(
            AppError.failure("Your session could not be renewed."),
          ),
        );
      } finally {
        isRefreshing = false;
      }
    }

    const errorData = error.response?.data as ApiErrorResponse;
    if (errorData && errorData.messages) {
      const errorMessage = errorData.messages.join(", ");
      const status = error.response?.status;
      if (status === 404)
        return Promise.reject(Result.failure(AppError.notFound(errorMessage)));
      if (status === 409)
        return Promise.reject(Result.failure(AppError.conflict(errorMessage)));
      if (status === 400)
        return Promise.reject(
          Result.failure(AppError.validation(errorMessage)),
        );
      return Promise.reject(Result.failure(AppError.failure(errorMessage)));
    }

    return Promise.reject(
      Result.failure(
        AppError.failure(error.message || "An unknown error occurred."),
      ),
    );
  },
);

export async function handleRequest<T>(
  request: Promise<any>,
): Promise<Result<T>> {
  try {
    const response = await request;
    if (response.data && typeof response.data.succeeded === "boolean") {
      return Result.success(response.data.data);
    }
    return Result.success(response.data);
  } catch (error) {
    return error as Result<T>;
  }
}

export default http;
