import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { Category } from "@/models/category";
import { Filters, AdvancedFilters } from "@/types/requests/category";
import { ApiResponse, PaginationResponse } from "@/types/responses/api";

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

export const useCategoryStore = defineStore("category", {
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
    items: [] as Category[],
    category: null as Category | null,
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },
    setFilters(filters: AdvancedFilters) {
      this.filters = { ...this.filters, ...filters };
    },

    async getAllItems() {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Category[]>(
          http.get("/v1/categories/all"),
        );

        if (result.succeeded && result.data) {
          this.items = result.data;
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

    async getPaginatedItems() {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<PaginationResponse<Category>>(
          http.get("/v1/categories", {
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

    async getById(id: string): Promise<Result<Category>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Category>(
          http.get(`/v1/categories/${id}`),
        );

        if (result.succeeded && result.data) {
          this.category = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.category = null;
        return error as Result<Category>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(category: Category): Promise<Result<Category>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Category>(
          http.put(`/v1/categories/${category.id}`, category),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Category>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(category: Category): Promise<Result<Category>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Category>(
          http.post("/v1/categories", category),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Category>;
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
          http.delete(`/v1/categories/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex(
            (item: Category) => item.id === id,
          );
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
