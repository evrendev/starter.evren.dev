import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { CourseEnrollmentDto } from "@/types/responses/courseEnrollment";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

export const useCourseEnrollmentStore = defineStore("courseEnrollment", {
  state: () => ({
    loading: false as boolean,
    error: null as AppError | null,
    enrollments: [] as CourseEnrollmentDto[],
  }),
  actions: {
    async enrollInCourse(courseId: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post("/v1/courseenrollments", { courseId }),
        );

        if (!result.succeeded) {
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

    async getMyEnrollments(): Promise<Result<CourseEnrollmentDto[]>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<CourseEnrollmentDto[]>(
          http.get("/v1/courseenrollments/mine"),
        );

        if (result.succeeded && result.data) {
          this.enrollments = result.data;
        } else {
          this.error = result.errors!;
          this.enrollments = [];
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        this.enrollments = [];
        return error as Result<CourseEnrollmentDto[]>;
      } finally {
        this.loading = false;
      }
    },
  },
});
