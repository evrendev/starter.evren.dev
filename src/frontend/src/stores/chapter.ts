import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { Chapter } from "@/models/chapter";
import { Filters, AdvancedFilters } from "@/types/requests/chapter";
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
  courseId: null,
};

export const useChapterStore = defineStore("chapter", {
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
    items: [] as Chapter[],
    chapter: null as Chapter | null,
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
        const result = await handleRequest<PaginationResponse<Chapter>>(
          http.get("/v1/chapters", {
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

    async getById(id: string): Promise<Result<Chapter>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Chapter>(
          http.get(`/v1/chapters/${id}`),
        );

        if (result.succeeded && result.data) {
          this.chapter = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.chapter = null;
        return error as Result<Chapter>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(chapter: Chapter): Promise<Result<Chapter>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Chapter>(
          http.put(`/v1/chapters/${chapter.id}`, chapter),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Chapter>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(chapter: Chapter): Promise<Result<Chapter>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Chapter>(
          http.post("/v1/chapters", chapter),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Chapter>;
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
          http.delete(`/v1/chapters/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item: Chapter) => item.id === id);
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
