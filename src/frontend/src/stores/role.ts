import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { Role } from "@/models/role";
import { Filters, AdvancedFilters } from "@/types/requests/role";
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

export const useRoleStore = defineStore("role", {
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
    items: [] as Role[],
    role: null as Role | null,
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
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
        const result = await handleRequest<PaginationResponse<Role>>(
          http.get("/roles", {
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

    async getById(id: string): Promise<Result<Role>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Role>(http.get(`/roles/${id}`));

        if (result.succeeded && result.data) {
          this.role = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.role = null;
        return error as Result<Role>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async getRolePermissions(id: string): Promise<Result<Role>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Role>(
          http.get(`/roles/${id}/permissions`),
        );

        if (result.succeeded && result.data) {
          this.role = result.data;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        this.role = null;
        return error as Result<Role>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async updateRolePermissions(role: Role): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.put(`/roles/${role.id}/permissions`, role),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<string>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(role: Role): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.put(`/roles/${role.id}`, role),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<string>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(role: Role): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(http.post("/roles", role));

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<string>;
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
          http.delete(`/roles/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item: Role) => item.id === id);
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
