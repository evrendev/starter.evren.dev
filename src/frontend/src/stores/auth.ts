import { ref, computed } from "vue";
import { defineStore } from "pinia";
import { Result } from "@/primitives/result";
import { AppError } from "@/primitives/error";
import { useHttpClient } from "@/composables/useHttpClient";
import type { User } from "@/models/user";
import type { LoginRequest } from "@/requests/auth";
import type { AccessTokenResponse, UserResponse } from "@/responses/auth";
import type { AxiosError, AxiosResponse } from "axios";
import Mapper from "@/mappers";

const DEFAULT_LANGUAGE = import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string;

const nullUser: User = {
  id: "",
  gender: "none",
  language: DEFAULT_LANGUAGE,
  firstName: "",
  lastName: "",
  fullName: "",
  initial: "",
  twoFactorEnabled: false,
  email: "",
  permissions: [],
};

export const useAuthStore = defineStore("auth", () => {
  const user = ref<User | undefined>(undefined);
  const accessToken = ref<string>(localStorage.getItem("accessToken") || "");
  const refreshToken = ref<string>(localStorage.getItem("refreshToken") || "");
  const refreshTokenExpiryTime = ref<Date | null>(
    localStorage.getItem("refreshTokenExpiryTime")
      ? new Date(localStorage.getItem("refreshTokenExpiryTime")!)
      : null,
  );

  const isAuthenticated = computed(
    () => !isLoading.value && user?.value?.id !== nullUser.id,
  );
  const isLoading = computed(() => user?.value === undefined);
  const permissions = computed(() => user?.value?.permissions ?? []);

  /**
   * Checks if the user has a specific permission.
   * @param {string} permission
   * @returns {boolean}
   */
  function hasPermission(permission: string | string[]): boolean {
    if (Array.isArray(permission)) {
      return permission.every((perm) => permissions.value.includes(perm));
    }

    return permissions.value.includes(permission);
  }

  /**
   * Clears the user state and local storage.
   * This function is used to reset the authentication state.
   */
  function clearStateAndStorage() {
    user.value = nullUser;
    accessToken.value = "";
    refreshToken.value = "";
    refreshTokenExpiryTime.value = null;

    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("refreshTokenExpiryTime");
  }

  /**
   * Sets the access token, refresh token, and refresh token expiry time.
   * @param {AccessTokenResponse} tokenData
   */
  function setTokens(tokenData: AccessTokenResponse) {
    accessToken.value = tokenData.accessToken || "";
    refreshToken.value = tokenData.refreshToken || "";
    refreshTokenExpiryTime.value = new Date(
      tokenData.refreshTokenExpiryTime || Date.now() + 3600000,
    );

    localStorage.setItem("accessToken", accessToken.value);
    localStorage.setItem("refreshToken", refreshToken.value);
    localStorage.setItem(
      "refreshTokenExpiryTime",
      refreshTokenExpiryTime.value?.toISOString() || "",
    );
  }

  async function login(
    values: LoginRequest,
  ): Promise<Result<AccessTokenResponse>> {
    try {
      const { data } = await useHttpClient().post<
        LoginRequest,
        AxiosResponse<Result<AccessTokenResponse>>
      >("auth/login", values);

      accessToken.value = data.data?.accessToken ?? "";
      refreshToken.value = data.data?.refreshToken ?? "";
      refreshTokenExpiryTime.value = new Date(
        data.data?.refreshTokenExpiryTime ?? Date.now() + 3600000,
      );

      if (!data.succeeded || !data.data) {
        return Result.failure(
          AppError.failure(
            data.errors?.message || "Invalid response from server",
          ),
        );
      }

      setTokens(data.data);
      await getUserInfo();

      return Result.success(data.data);
    } catch (error) {
      const apiError = error as AxiosError;
      return Result.failure(AppError.failure(apiError.message));
    }
  }

  async function logout(): Promise<Result<string>> {
    try {
      await useHttpClient().post("auth/logout");
      return Result.success("Logout successful");
    } catch (error) {
      console.error("Logout failed:", error);
      return Result.failure(AppError.failure("Logout failed"));
    } finally {
      clearStateAndStorage();
    }
  }

  async function getUserInfo(): Promise<Result<string>> {
    try {
      const { data } =
        await useHttpClient().get<UserResponse>("personal/profile");
      const permissions = await useHttpClient().get<string[]>(
        "personal/permissions",
      );
      user.value = Mapper.toUser(data);
      user.value.permissions = permissions.data;
    } catch (error: unknown) {
      user.value = nullUser;
      const apiError = error as AxiosError;
      return Result.failure(AppError.failure(apiError.message));
    }

    return Result.success("User information retrieved successfully");
  }

  async function refresh(): Promise<Result<string>> {
    try {
      const { data } =
        await useHttpClient().get<AxiosResponse<AccessTokenResponse>>(
          "auth/refresh-token",
        );

      if (data.status !== 200 || !data.data) {
        await logout();
        return Result.failure(AppError.failure("Could not refresh token"));
      }

      setTokens(data.data);

      return Result.success(data.data?.accessToken ?? "");
    } catch (error) {
      const apiError = error as AxiosError;
      await logout();
      return Result.failure(AppError.failure(apiError.message));
    }
  }

  async function initializeStore(): Promise<void> {
    if (
      !accessToken.value ||
      !refreshToken.value ||
      !refreshTokenExpiryTime.value
    ) {
      user.value = nullUser;
      return;
    }

    if (refreshTokenExpiryTime.value <= new Date()) {
      await logout();
      return;
    }

    await getUserInfo();
  }

  return {
    // State
    user,
    accessToken,
    refreshToken,
    isLoading,
    // Getters
    isAuthenticated,
    permissions,
    // Actions
    login,
    logout,
    getUserInfo,
    hasPermission,
    refresh,
    initializeStore,
  };
});
