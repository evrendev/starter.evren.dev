import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import type { BasicUser, Log, User } from "@/models/user";
import type { ChangePasswordRequest } from "@/types/requests/user";
import type { SetupTwoFactorAuthenticationResponse } from "@/types/responses/personal";
import type {
  EnableTwoFactorAuthenticationRequest,
  LogFilters,
  SetupTwoFactorAuthenticationRequest,
} from "@/types/requests/personal";
import type { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import type { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

const DEFAULT_FILTER: LogFilters = {
  search: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
  startDate: null,
  endDate: null,
};

export const usePersonalStore = defineStore("personal", {
  state: () => ({
    loading: false as boolean,
    // Add error state for reactive error handling
    error: null as AppError | null,
    // Pagination state
    page: 1,
    totalPages: 0,
    total: 0,
    itemsPerPage: 25,
    hasNextPage: false,
    hasPreviousPage: false,
    items: [] as Log[],
    // Data state
    user: null as BasicUser | null,
    permissions: [] as string[],
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
    clearProfile() {
      this.user = {} as BasicUser;

      this.permissions = [];
    },

    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },

    setFilters(filters: LogFilters) {
      this.filters = { ...this.filters, ...filters };
    },

    async getUser() {
      this.loading = true;
      this.error = null; // İşleme başlarken hatayı temizle
      const appStore = useAppStore();
      appStore.setLoading(true);

      // try/catch yerine handleRequest kullanıyoruz
      const result = await handleRequest<User>(http.get("personal/profile"));

      if (result.succeeded && result.data) {
        this.user = result.data;
      } else {
        this.error = result.errors!;
      }

      this.loading = false;
      appStore.setLoading(false);
    },

    async getPermissions() {
      this.loading = true;
      this.error = null;

      const result = await handleRequest<string[]>(
        http.get("personal/permissions"),
      );

      if (result.succeeded && result.data) {
        this.permissions = result.data;
      } else {
        this.error = result.errors!;
        this.permissions = []; // Hata durumunda izinleri temizle
      }

      this.loading = false;
    },

    hasPermission(permission: string | string[]): boolean {
      if (!this.permissions || this.permissions.length === 0) {
        return false;
      }

      if (Array.isArray(permission)) {
        return permission.every((perm) => this.permissions.includes(perm));
      }

      return this.permissions.includes(permission);
    },

    async update(user: BasicUser): Promise<Result<User>> {
      this.loading = true;
      this.error = null;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const result = await handleRequest<User>(
        http.put("personal/profile", user),
      );

      if (result.succeeded && result.data) {
        this.user = result.data;
      } else {
        this.error = result.errors!;
      }

      this.loading = false;
      appStore.setLoading(false);
      return result; // Component'in sonucu bilmesi için result'ı döndür
    },

    async changePassword(
      values: ChangePasswordRequest,
    ): Promise<Result<string>> {
      this.loading = true;
      this.error = null;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const result = await handleRequest<string>(
        http.put("personal/change-password", values),
      );

      if (result.isFailure) {
        this.error = result.errors!;
      }

      this.loading = false;
      appStore.setLoading(false);
      return result;
    },

    async getLogs() {
      this.loading = true;
      this.error = null;

      const result = await handleRequest<PaginationResponse<Log>>(
        http.get("/personal/logs", { params: this.filters }),
      );

      if (result.succeeded && result.data) {
        this.items = result.data.items;
        this.page = result.data.page;
        this.total = result.data.total;
        this.itemsPerPage = result.data.itemsPerPage;
        this.totalPages = result.data.totalPages;
        this.hasNextPage = result.data.hasNextPage;
        this.hasPreviousPage = result.data.hasPreviousPage;
      } else {
        this.error = result.errors!;
        this.items = [];
      }

      this.loading = false;
    },

    async setupTwoFactorAuthentication(): Promise<
      Result<SetupTwoFactorAuthenticationResponse>
    > {
      if (!this.user) throw new Error("User not loaded");

      this.loading = true;
      this.error = null;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const request: SetupTwoFactorAuthenticationRequest = {
        id: this.user.id as string,
      };
      const result = await handleRequest<SetupTwoFactorAuthenticationResponse>(
        http.get("2fa/setup", { params: request }),
      );

      if (result.isFailure) this.error = result.errors!;

      this.loading = false;
      appStore.setLoading(false);
      return result;
    },

    async enableTwoFactorAuthentication(
      request: EnableTwoFactorAuthenticationRequest,
    ): Promise<Result<string[]>> {
      if (!this.user) throw new Error("User not loaded");

      request.id = this.user.id as string;
      this.loading = true;
      this.error = null;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const result = await handleRequest<string[]>(
        http.post("2fa/enable", request),
      );

      if (result.succeeded) {
        this.user.twoFactorEnabled = true;
      } else {
        this.error = result.errors!;
      }

      this.loading = false;
      appStore.setLoading(false);
      return result;
    },

    async disableTwoFactorAuthentication(): Promise<Result<boolean>> {
      if (!this.user) throw new Error("User not loaded");

      const request: SetupTwoFactorAuthenticationRequest = {
        id: this.user.id as string,
      };
      this.loading = true;
      this.error = null;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const result = await handleRequest<boolean>(
        http.post("2fa/disable", request),
      );

      if (result.succeeded) {
        this.user.twoFactorEnabled = false;
      } else {
        this.error = result.errors!;
      }

      this.loading = false;
      appStore.setLoading(false);
      return result;
    },
  },
});
