import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { LessonPage, LessonPageDetails, LessonPageProgress } from "@/models/lessonPage";
import { PaginateLessonPagesFilter, AdvancedFilters, CreateLessonPageRequest, UpdateLessonPageRequest } from "@/types/requests/lessonPage";
import { LessonPageDto, LessonPlayerDto, LessonPlayerPageDto } from "@/types/responses/lessonPage";
import { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

const DEFAULT_FILTER: PaginateLessonPagesFilter = {
  search: null,
  lessonId: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const useLessonPageStore = defineStore("lessonPage", {
  state: () => ({
    loading: false as boolean,
    error: null as AppError | null,
    // Pagination state
    page: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    total: 0 as number,
    itemsPerPage: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,
    // Data state
    items: [] as LessonPageDto[],
    lessonPage: null as LessonPageDetails | null,
    filters: { ...DEFAULT_FILTER },
    // Player state
    currentPage: null as LessonPlayerDto | null,
    pages: [] as LessonPlayerPageDto[],
    progressPercent: 0 as number,
    lastVisitedPageId: null as string | null,
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },

    setFilters(filters: Partial<PaginateLessonPagesFilter>) {
      this.filters = { ...this.filters, ...filters };
    },

    async getPaginatedPages(lessonId: string, filters?: Partial<AdvancedFilters>) {
      this.loading = true;
      this.error = null;

      try {
        const pageFilters = { ...this.filters, lessonId, ...filters };
        const result = await handleRequest<PaginationResponse<LessonPageDto>>(
          http.get("/v1/lessonpages", {
            params: pageFilters,
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

    async getPageById(id: string): Promise<Result<LessonPageDetails>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<LessonPageDetails>(
          http.get(`/v1/lessonpages/${id}`),
        );

        if (result.succeeded && result.data) {
          this.lessonPage = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.lessonPage = null;
        return error as Result<LessonPageDetails>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async createPage(payload: CreateLessonPageRequest): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.post("/v1/lessonpages", payload),
        );

        if (!result.succeeded) {
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

    async updatePage(id: string, payload: UpdateLessonPageRequest): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.put(`/v1/lessonpages/${id}`, payload),
        );

        if (!result.succeeded) {
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

    async deletePage(id: string): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.delete(`/v1/lessonpages/${id}`),
        );

        if (!result.succeeded) {
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

    async getLessonPlayer(lessonId: string): Promise<Result<LessonPlayerDto>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<LessonPlayerDto>(
          http.get(`/v1/lessonpages/${lessonId}/player`),
        );

        if (result.succeeded && result.data) {
          this.currentPage = result.data;
          this.pages = result.data.pages || [];
          this.progressPercent = result.data.percentComplete || 0;
          this.lastVisitedPageId = result.data.lastVisitedPageId || null;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<LessonPlayerDto>;
      } finally {
        this.loading = false;
      }
    },

    async markPageCompleted(pageId: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/v1/lessonpages/${pageId}/complete`),
        );

        if (result.succeeded) {
          // Update local player state if available
          if (this.currentPage) {
            const pageIndex = this.currentPage.pages.findIndex((p) => p.id === pageId);
            if (pageIndex >= 0) {
              // Note: pages array structure depends on backend response
              // This is a placeholder for state update
            }
          }
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
