// stores/auth.ts
import { defineStore } from "pinia";
import type { AccessTokenResponse } from "@/responses/auth";
import { LoginRequest } from "@/requests/auth";
import { Result } from "@/primitives/result";
import { AxiosError, AxiosResponse } from "axios";
import { AppError } from "@/primitives/error";
import { useHttpClient } from "@/composables/useHttpClient";
import { useProfileStore } from "./profile";

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

        const profileStore = useProfileStore();
        await profileStore.getUser();
        await profileStore.getPermissions();

        return Result.success(data.data);
      } catch (error) {
        const apiError = error as AxiosError;
        return Result.failure(AppError.failure(apiError.message));
      }
    },
    async logout(): Promise<Result<void>> {
      const profileStore = useProfileStore();
      try {
        await useHttpClient().post("auth/logout");
      } catch (error) {
        console.error(
          "Server logout failed, clearing client state anyway:",
          error,
        );
      } finally {
        this.clearTokens();
        // GÜNCELLEME: Profile store'u da temizliyoruz
        profileStore.clearProfile();
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
        const apiError = error as AxiosError;

        await this.logout();

        return Result.failure(AppError.failure(apiError.message));
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
