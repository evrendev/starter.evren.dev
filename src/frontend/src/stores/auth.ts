import { defineStore } from "pinia";
import type { AccessTokenResponse } from "@/types/responses/auth";
import type { LoginRequest, TwoFactorAuthRequest } from "@/types/requests/auth";
import { Result } from "@/primitives/result";
import { usePersonalStore } from "./personal";
import http, { handleRequest } from "@/utils/http";
import { useAppStore } from "./app";
import { router } from "@/plugins/router";

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
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<AccessTokenResponse>(
          http.post("auth/login", values),
        );

        if (result.succeeded && result.data) {
          this.setTokens(result.data);

          if (!result.data.twoFactorAuthRequired) {
            const personalStore = usePersonalStore();
            await personalStore.getUser();
            await personalStore.getPermissions();
          }
        } else {
          this.clearTokens();
        }

        return result;
      } catch (error) {
        this.clearTokens();
        throw error;
      } finally {
        appStore.setLoading(false);
      }
    },

    async verifyTwoFactorAuth(
      values: TwoFactorAuthRequest,
    ): Promise<Result<AccessTokenResponse>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<AccessTokenResponse>(
          http.post("2fa/verify", values),
        );

        if (result.succeeded && result.data) {
          this.setTokens(result.data);
          const personalStore = usePersonalStore();
          await personalStore.getUser();
          await personalStore.getPermissions();
        }
        return result;
      } catch (error) {
        throw error;
      } finally {
        appStore.setLoading(false);
      }
    },

    async logout(): Promise<Result<void>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<void>(http.post("auth/logout"));
        return result;
      } catch (error) {
        this.clearTokens();
        throw error;
      } finally {
        const personalStore = usePersonalStore();
        this.clearTokens();
        personalStore.clearProfile();
        appStore.setLoading(false);
        window.location.href = "/auth/login";
      }
    },

    async refresh(): Promise<Result<AccessTokenResponse>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<AccessTokenResponse>(
          http.get("auth/refresh-token"),
        );

        if (result.succeeded && result.data) {
          this.setTokens(result.data);
        } else {
          await this.logout();
        }
        return result;
      } catch (error) {
        await this.logout();
        throw error;
      } finally {
        appStore.setLoading(false);
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
