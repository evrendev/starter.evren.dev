import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import {
  StudentSummary,
  StudentsSummaryStats,
  StudentDetail,
} from "@/models/student";
import { Filters, AdvancedFilters } from "@/types/requests/student";
import { PaginationResponse } from "@/types/responses/api";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

const DEFAULT_FILTER: Filters = {
  search: null,
  isActive: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const useStudentStore = defineStore("student", {
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
    items: [] as StudentSummary[],
    student: null as StudentDetail | null,
    summaryStats: null as StudentsSummaryStats | null,
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
        const result = await handleRequest<PaginationResponse<StudentSummary>>(
          http.get("/students", {
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

    async getSummaryStats(): Promise<Result<StudentsSummaryStats>> {
      try {
        const result = await handleRequest<StudentsSummaryStats>(
          http.get("/students/summary-stats"),
        );

        if (result.succeeded && result.data) {
          this.summaryStats = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<StudentsSummaryStats>;
      }
    },

    async getDetail(userId: string): Promise<Result<StudentDetail>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<StudentDetail>(
          http.get(`/students/${userId}`),
        );

        if (result.succeeded && result.data) {
          this.student = result.data;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        this.student = null;
        return error as Result<StudentDetail>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async enrollStudent(
      userId: string,
      courseId: string,
    ): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<boolean>(
          http.post(`/students/${userId}/enrollments/${courseId}`),
        );

        if (!result.succeeded) {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        appStore.setLoading(false);
      }
    },

    async unenrollStudent(
      userId: string,
      courseId: string,
    ): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<boolean>(
          http.delete(`/students/${userId}/enrollments/${courseId}`),
        );

        if (!result.succeeded) {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        appStore.setLoading(false);
      }
    },

    // Reuses the existing Users toggle-status endpoint (Task O0/O1) — Students
    // deliberately doesn't get its own activate/deactivate endpoint, see Task
    // R1's design.
    async toggleStatus(
      userId: string,
      activateUser: boolean,
    ): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const result = await handleRequest<boolean>(
          http.post(`/users/${userId}/toggle-status`, {
            userId,
            activateUser,
          }),
        );

        if (result.succeeded) {
          const item = this.items.find((s) => s.userId === userId);
          if (item) item.isActive = activateUser;
          if (this.student && this.student.userId === userId)
            this.student.isActive = activateUser;
        } else {
          this.error = result.errors!;
        }
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        appStore.setLoading(false);
      }
    },
  },
});
