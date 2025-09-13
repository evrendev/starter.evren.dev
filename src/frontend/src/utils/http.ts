import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from "axios";
import { Result } from "@/primitives/result";
import { AppError } from "@/primitives/error";
import type { ApiErrorResponse } from "@/types/responses/api";
import { useAuthStore } from "@/stores/auth"; // Auth store'u import ediyoruz

// --- Temel Axios Konfigürasyonu ---
const API_BASE_URL = import.meta.env.VITE_APP_API_BASE_URL as string;
const http: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// --- Birleştirilmiş Request Interceptor ---
// Tenant, Dil ve Auth Token'ı tek bir yerden ekliyoruz.
http.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Auth Store'u interceptor içinde çağırıyoruz.
    // Bu, modül döngüsü sorunlarını engeller.
    const authStore = useAuthStore();

    // Tenant ve Dil Bilgisi
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

    // Authorization Token'ı ekleme
    if (authStore.accessToken && !config.headers["Authorization"]) {
      config.headers["Authorization"] = `Bearer ${authStore.accessToken}`;
    }

    return config;
  },
  (error) => Promise.reject(error),
);

// --- Birleştirilmiş ve Geliştirilmiş Response Interceptor ---
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
    // Return the original response object for axios compatibility
    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as InternalAxiosRequestConfig & {
      _retry?: boolean;
    };
    const authStore = useAuthStore();

    // 401 (Unauthorized) hatası geldiğinde ve token yenileme koşulları sağlandığında
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      authStore.refreshToken
    ) {
      if (isRefreshing) {
        // Eğer zaten bir token yenileme işlemi varsa, bu isteği sıraya al
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
        const result = await authStore.refresh(); // Auth store'daki refresh fonksiyonu
        if (result.succeeded) {
          processQueue(null, authStore.accessToken);
          originalRequest.headers!["Authorization"] =
            `Bearer ${authStore.accessToken}`;
          return http(originalRequest); // Orijinal isteği yeni token ile tekrarla
        } else {
          processQueue(result.errors, null);
          authStore.logout(); // Refresh başarısız olursa logout yap
          return Promise.reject(Result.failure(result.errors!));
        }
      } catch (refreshError) {
        processQueue(refreshError, null);
        authStore.logout();
        return Promise.reject(
          Result.failure(AppError.failure("Oturumunuz yenilenemedi.")),
        );
      } finally {
        isRefreshing = false;
      }
    }

    // Diğer tüm hatalar için standart hata formatını döndür
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
        AppError.failure(error.message || "Bilinmeyen bir hata oluştu."),
      ),
    );
  },
);

// --- Yardımcı Fonksiyon (DEĞİŞİKLİK YOK) ---
// Store'larda try/catch'ten kurtulmak için.
export async function handleRequest<T>(
  request: Promise<any>,
): Promise<Result<T>> {
  try {
    // Get the axios response and transform it to Result
    const response = await request;
    if (response.data && typeof response.data.succeeded === "boolean") {
      return Result.success(response.data.data);
    }
    return Result.success(response.data);
  } catch (error) {
    // Interceptor'dan gelen Result.failure nesnesini yakalıyoruz.
    return error as Result<T>;
  }
}

export default http;
