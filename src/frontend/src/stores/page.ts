import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { PageDetails } from "@/models/page";
import { PaginatePagesFilter, AdvancedFilters, CreatePageRequest, UpdatePageRequest } from "@/types/requests/page";
import { PageDto, ChapterPlayerDto, ChapterPlayerPageDto } from "@/types/responses/page";
import { ImportJobDto } from "@/types/responses/importJob";
import { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

const DEFAULT_FILTER: PaginatePagesFilter = {
  search: null,
  chapterId: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const usePageStore = defineStore("page", {
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
    items: [] as PageDto[],
    pageDetails: null as PageDetails | null,
    filters: { ...DEFAULT_FILTER },
    // Player state
    currentPage: null as ChapterPlayerDto | null,
    pages: [] as ChapterPlayerPageDto[],
    progressPercent: 0 as number,
    lastVisitedPageId: null as string | null,
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },

    setFilters(filters: Partial<PaginatePagesFilter>) {
      this.filters = { ...this.filters, ...filters };
    },

    async getPaginatedPages(chapterId: string, filters?: Partial<AdvancedFilters>) {
      this.loading = true;
      this.error = null;

      try {
        const pageFilters = { ...this.filters, chapterId, ...filters };
        const result = await handleRequest<PaginationResponse<PageDto>>(
          http.get("/v1/pages", {
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

    async getPageById(id: string): Promise<Result<PageDetails>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<PageDetails>(
          http.get(`/v1/pages/${id}`),
        );

        if (result.succeeded && result.data) {
          this.pageDetails = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.pageDetails = null;
        return error as Result<PageDetails>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async createPage(payload: CreatePageRequest): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.post("/v1/pages", payload),
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

    async updatePage(id: string, payload: UpdatePageRequest): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.put(`/v1/pages/${id}`, payload),
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
          http.delete(`/v1/pages/${id}`),
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

    async getChapterPlayer(chapterId: string): Promise<Result<ChapterPlayerDto>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<ChapterPlayerDto>(
          http.get(`/v1/pages/${chapterId}/player`),
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
        return error as Result<ChapterPlayerDto>;
      } finally {
        this.loading = false;
      }
    },

    async markPageCompleted(pageId: string): Promise<Result<boolean>> {
      // Background call fired on slide change — must not toggle loading,
      // or the player's v-if would tear down the reveal.js slide DOM.
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/v1/pages/${pageId}/complete`),
        );

        if (result.succeeded) {
          const page = this.pages.find((p) => p.id === pageId);
          if (page && !page.completed) {
            page.completed = true;
            page.completedAt = new Date();
            const completedCount = this.pages.filter((p) => p.completed).length;
            this.progressPercent = this.pages.length
              ? Math.round((completedCount * 100) / this.pages.length)
              : 0;
          }
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      }
    },

    // Deliberately NOT using handleRequest<T>: the request body is FormData (multipart),
    // not JSON. We build FormData ourselves and let the browser set the Content-Type
    // (with the multipart boundary) instead of the axios instance's default
    // "application/json" header.
    async importPptx(
      courseId: string,
      file: File,
      slidesHtml?: string[],
    ): Promise<Result<string>> {
      this.loading = true;
      this.error = null;

      try {
        const formData = new FormData();
        formData.append("file", file);
        // Rich, client-parsed per-slide HTML (see usePptxSlidesParser) — optional,
        // the backend falls back to its own plain-text extraction when absent
        if (slidesHtml) {
          formData.append("slidesHtml", JSON.stringify(slidesHtml));
        }

        const response = await http.post<string>(
          `/v1/courses/${courseId}/import-pptx`,
          formData,
          { headers: { "Content-Type": undefined } },
        );

        return Result.success(response.data);
      } catch (error) {
        this.error = error as AppError;
        return error as Result<string>;
      } finally {
        this.loading = false;
      }
    },

    async getImportJobStatus(jobId: string): Promise<Result<ImportJobDto>> {
      return handleRequest<ImportJobDto>(
        http.get(`/v1/pages/import/${jobId}/status`),
      );
    },
  },
});
