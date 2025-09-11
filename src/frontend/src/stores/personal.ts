import { AxiosResponse } from "axios";
import { BasicUser, Log, User } from "@/models/user";
import { DefaultApiResponse, PaginationResponse } from "@/responses/api";
import { useHttpClient } from "@/composables/useHttpClient";
import { defineStore } from "pinia";
import { useAppStore } from "./app";
import Mapper from "@/mappers";
import { ChangePasswordRequest, LogFilters } from "@/requests/user";
import {
  DisableTwoFactorAuthenticationRequest,
  EnableTwoFactorAuthenticationRequest,
  SetupTwoFactorAuthenticationRequest,
} from "@/requests/personal";
import { SetupTwoFactorAuthenticationResponse } from "@/responses/personal";

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
    page: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    total: 0 as number,
    itemsPerPage: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,
    items: [] as Log[],
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
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().get<User>("personal/profile");
        this.user = Mapper.toUser(data);
      } catch (error: unknown) {
        console.error("Failed to fetch profile:", error);
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async getPermissions() {
      this.loading = true;
      try {
        const { data } = await useHttpClient().get<string[]>(
          "personal/permissions",
        );
        this.permissions = data || [];
      } catch (error: unknown) {
        console.error("Failed to fetch permissions:", error);
        this.permissions = [];
      } finally {
        this.loading = false;
      }
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
    async update(user: BasicUser) {
      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put<DefaultApiResponse<User>>(
          "personal/profile",
          user,
        );

        this.user = Mapper.toUser(data.data);
        return data;
      } catch (error: unknown) {
        console.error("Failed to update profile:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async changePassword(values: ChangePasswordRequest) {
      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put<DefaultApiResponse<string>>(
          "personal/change-password",
          values,
        );

        return data;
      } catch (error: unknown) {
        console.error("Failed to change password:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async getLogs() {
      this.loading = true;

      try {
        const { data }: AxiosResponse<PaginationResponse<Log>> =
          await useHttpClient().get("/personal/logs", {
            params: this.filters,
          });

        this.items = data.items;
        this.page = data.page;
        this.total = data.total;
        this.itemsPerPage = data.itemsPerPage;
        this.totalPages = data.totalPages;
        this.hasNextPage = data.hasNextPage;
        this.hasPreviousPage = data.hasPreviousPage;
      } catch (error) {
        console.error("Error fetching items:", error);
        return [];
      } finally {
        this.loading = false;
      }
    },
    async setupTwoFactorAuthentication() {
      if (!this.user) throw new Error("User not loaded");

      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      const request: SetupTwoFactorAuthenticationRequest = {
        id: this.user.id as string,
      };

      try {
        const { data } = await useHttpClient().get<
          DefaultApiResponse<SetupTwoFactorAuthenticationResponse>
        >("2fa/setup", {
          params: request,
        });

        return data;
      } catch (error: unknown) {
        console.error("Failed to setup two-factor authentication:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async enableTwoFactorAuthentication(
      request: EnableTwoFactorAuthenticationRequest,
    ) {
      if (!this.user) throw new Error("User not loaded");

      request.id = this.user.id as string;
      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().post<
          DefaultApiResponse<string[]>
        >("2fa/enable", request);

        this.user.twoFactorEnabled = true;
        return data;
      } catch (error: unknown) {
        console.error("Failed to enable two-factor authentication:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async disableTwoFactorAuthentication() {
      if (!this.user) throw new Error("User not loaded");

      const request: SetupTwoFactorAuthenticationRequest = {
        id: this.user.id as string,
      };
      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().post<
          DefaultApiResponse<boolean>
        >("2fa/disable", request);

        this.user.twoFactorEnabled = false;
        return data;
      } catch (error: unknown) {
        console.error("Failed to disable two-factor authentication:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
