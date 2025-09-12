// stores/auth.ts
import { defineStore } from "pinia";
import type { AccessTokenResponse } from "@/responses/auth";
import { LoginRequest, TwoFactorAuthRequest } from "@/requests/auth";
import { Result } from "@/primitives/result";
import { AxiosError, AxiosResponse } from "axios";
import { AppError } from "@/primitives/error";
import { useHttpClient } from "@/composables/useHttpClient";
import { usePersonalStore } from "./personal";
import { ErrorResponse } from "@/responses/api";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    accessToken: localStorage.getItem("accessToken") || null,
    refreshToken: localStorage.getItem("refreshToken") || null,
    refreshTokenExpiryTime: localStorage.getItem("refreshTokenExpiryTime")
      ? new Date(localStorage.getItem("refreshTokenExpiryTime")!)
      : null,
  }),
  getters: {
    isAuthenticated: (state): boolean =>
      !!state.accessToken &&
      !!state.refreshToken &&
      !!state.refreshTokenExpiryTime &&
      state.refreshTokenExpiryTime > new Date(),
  },
  actions: {
    clearTokens() {
      this.accessToken = null;
      this.refreshToken = null;
      this.refreshTokenExpiryTime = null;

      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("refreshTokenExpiryTime");
    },
    setTokens(tokenData: AccessTokenResponse) {
      this.accessToken = tokenData.accessToken ?? "";
      this.refreshToken = tokenData.refreshToken ?? "";
      this.refreshTokenExpiryTime = new Date(
        tokenData.refreshTokenExpiryTime ?? new Date(),
      );

      localStorage.setItem("accessToken", this.accessToken);
      localStorage.setItem("refreshToken", this.refreshToken);
      localStorage.setItem(
        "refreshTokenExpiryTime",
        this.refreshTokenExpiryTime.toISOString(),
      );
    },
    async login(values: LoginRequest): Promise<Result<AccessTokenResponse>> {
      try {
        const { data } = await useHttpClient().post<
          LoginRequest,
          AxiosResponse<Result<AccessTokenResponse>>
        >("auth/login", values);

        if (!data.succeeded || !data.data) {
          return Result.failure(
            AppError.failure(
              Array.isArray(data.errors)
                ? data.errors[0]
                : data.errors || "Invalid response from server",
            ),
          );
        }

        this.setTokens(data.data);

        if (!data.data.twoFactorAuthRequired) {
          const personalStore = usePersonalStore();
          await personalStore.getUser();
          await personalStore.getPermissions();
        }

        return Result.success(data.data);
      } catch (error) {
        const apiError = error as AxiosError<ErrorResponse>;
        console.error("Login error:", apiError);
        return Result.failure(
          AppError.failure(
            apiError.response?.data?.messages.join(", ") || apiError.message,
          ),
        );
      }
    },
    async verifyTwoFactorAuth(
      values: TwoFactorAuthRequest,
    ): Promise<Result<AccessTokenResponse>> {
      try {
        const { data } = await useHttpClient().post<
          TwoFactorAuthRequest,
          AxiosResponse<Result<AccessTokenResponse>>
        >("2fa/verify", values);

        if (!data.succeeded || !data.data) {
          return Result.failure(
            AppError.failure(
              Array.isArray(data.errors)
                ? data.errors[0]
                : data.errors || "Invalid response from server",
            ),
          );
        }

        const personalStore = usePersonalStore();
        await personalStore.getUser();
        await personalStore.getPermissions();

        return Result.success(data.data);
      } catch (error) {
        const apiError = error as AxiosError<ErrorResponse>;
        console.error("Login error:", apiError);
        return Result.failure(
          AppError.failure(
            apiError.response?.data?.messages.join(", ") || apiError.message,
          ),
        );
      }
    },
    async logout(): Promise<Result<void>> {
      const personalStore = usePersonalStore();
      try {
        await useHttpClient().post("auth/logout");
      } catch (error) {
        const apiError = error as AxiosError<ErrorResponse>;
        console.error("Logout error:", apiError);
        return Result.failure(
          AppError.failure(
            apiError.response?.data?.messages.join(", ") || apiError.message,
          ),
        );
      } finally {
        this.clearTokens();
        personalStore.clearProfile();
      }

      return Result.success<void>(undefined);
    },
    async refresh(): Promise<Result<string>> {
      try {
        const { data } =
          await useHttpClient().get<AxiosResponse<AccessTokenResponse>>(
            "auth/refresh-token",
          );

        if (data.status === 401) {
          await this.logout();

          return Result.failure(AppError.failure("Could not refresh token"));
        }

        this.setTokens(data.data);

        return Result.success(data.data?.accessToken ?? "");
      } catch (error) {
        await this.logout();
        const apiError = error as AxiosError<ErrorResponse>;
        console.error("Refresh token error:", apiError);
        return Result.failure(
          AppError.failure(
            apiError.response?.data?.messages.join(", ") || apiError.message,
          ),
        );
      }
    },
    async initializeStore(): Promise<void> {
      if (
        this.refreshTokenExpiryTime != null &&
        this.refreshTokenExpiryTime <= new Date()
      ) {
        await this.logout();

        return;
      }
    },
  },
});
