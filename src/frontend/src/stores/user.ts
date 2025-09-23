import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { User } from "@/models/user";
import {
  ResetPasswordRequest,
  Filters,
  AdvancedFilters,
} from "@/types/requests/user";
import { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

const DEFAULT_FILTER: Filters = {
  search: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const useUserStore = defineStore("user", {
  state: () => ({
    loading: false as boolean,
    // Add error state for reactive error handling
    error: null as AppError | null,
    // Pagination state
    page: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    total: 0 as number,
    itemsPerPage: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,
    // Data state
    items: [] as User[],
    user: null as User | null,
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
    async forgotPassword(email: string): Promise<string> {
      try {
        const { data } = await http.post("/users/forgot-password", {
          email,
        });
        return data;
      } catch (error) {
        console.error("Forgot password error:", error);
        throw error;
      }
    },
    async resetPassword(data: ResetPasswordRequest): Promise<string> {
      try {
        const { data: response } = await http.post(
          "/users/reset-password",
          data,
        );
        return response;
      } catch (error) {
        console.error("Reset password error:", error);
        throw error;
      }
    },
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },
    setFilters(filters: AdvancedFilters) {
      this.filters = { ...this.filters, ...filters };
    },

    async getPaginatedItems() {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<PaginationResponse<User>>(
          http.get("/users", {
            params: this.filters,
          }),
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
      } catch (error) {
        this.error = error as AppError;
        this.items = [];
      } finally {
        this.loading = false;
      }
    },

    async getById(id: string): Promise<Result<User>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<User>(http.get(`/users/${id}`));

        if (result.succeeded && result.data) {
          this.user = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.user = null;
        return error as Result<User>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(user: User): Promise<Result<User>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<User>(
          http.put(`/users/${user.id}`, user),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<User>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(user: User): Promise<Result<User>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<User>(http.post("/users", user));

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<User>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async delete(id: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.delete(`/users/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item: User) => item.id === id);
          if (index !== -1) this.items.splice(index, 1);
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
