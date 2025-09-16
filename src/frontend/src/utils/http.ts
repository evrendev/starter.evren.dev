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
let retryCount: number = 0;
const MAX_RETRIES = 3;

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

http.interceptors.response.use(
  (response) => response,
  async (error: any) => {
    const authStore = useAuthStore();

    const prevRequest = error?.config;
    if (
      error?.response?.status === 401 &&
      !prevRequest._retry &&
      authStore.refreshToken &&
      authStore.refreshToken.length > 0
    ) {
      if (retryCount >= MAX_RETRIES) {
        await authStore.logout();
        return Promise.reject(error);
      }

      prevRequest._retry = true;
      retryCount += 1;

      try {
        const result = await authStore.refresh();
        if (result.succeeded) {
          prevRequest.headers["Authorization"] =
            `Bearer ${authStore.accessToken}`;
          retryCount = 0;
          return http(prevRequest);
        } else {
          return Promise.reject(error);
        }
      } catch (error) {
        return Promise.reject(error);
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
