import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { Course } from "@/models/course";
import { Filters, BasicFilters } from "@/types/requests/course";
import { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";
import Mapper from "@/mappers";
import { convertToUploadRequest } from "@/utils/tools";

const DEFAULT_FILTER: Filters = {
  search: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
  categoryId: null,
  published: null,
};

export const useCourseStore = defineStore("course", {
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
    items: [] as Course[],
    course: null as Course | null,
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },

    setFilters(filters: BasicFilters) {
      this.filters = { ...this.filters, ...filters };
    },

    async getPaginatedItems() {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<PaginationResponse<Course>>(
          http.get("/v1/courses", {
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

    async getById(id: string): Promise<Result<Course>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Course>(
          http.get(`/v1/courses/${id}`),
        );

        if (result.succeeded && result.data) {
          this.course = await Mapper.toCourse(result.data);
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.course = null;
        return error as Result<Course>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(course: Course): Promise<Result<Course>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const uploadRequest = await convertToUploadRequest(course.image);

        const result = await handleRequest<Course>(
          http.put(`/v1/courses/${course.id}`, {
            ...course,
            image: uploadRequest,
          }),
        );

        if (!result.succeeded || !result.data) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Course>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(course: Course): Promise<Result<Course>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const uploadRequest = await convertToUploadRequest(course.image);

        const result = await handleRequest<Course>(
          http.post("/v1/courses", { ...course, image: uploadRequest }),
        );

        if (result.succeeded && result.data) {
          this.course = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Course>;
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
          http.delete(`/v1/courses/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item: Course) => item.id === id);
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
