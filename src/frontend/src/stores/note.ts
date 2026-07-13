import { defineStore } from "pinia";
import { useAppStore } from "./app";

// Local Types
import { NoteDto } from "@/types/responses/lessonPage";
import { CreateNoteRequest } from "@/types/requests/lessonPage";

// Refactored Architecture Imports
import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

export const useNoteStore = defineStore("note", {
  state: () => ({
    loading: false as boolean,
    error: null as AppError | null,
    notes: [] as NoteDto[],
    currentLessonPageId: null as string | null,
  }),
  actions: {
    async getNotesByLessonPage(lessonPageId: string): Promise<Result<NoteDto[]>> {
      this.loading = true;
      this.error = null;
      this.currentLessonPageId = lessonPageId;

      try {
        const result = await handleRequest<NoteDto[]>(
          http.get("/v1/notes", {
            params: { lessonPageId },
          }),
        );

        if (result.succeeded && result.data) {
          this.notes = result.data;
        } else {
          this.error = result.errors!;
          this.notes = [];
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        this.notes = [];
        return error as Result<NoteDto[]>;
      } finally {
        this.loading = false;
      }
    },

    async getNoteById(id: string): Promise<Result<NoteDto>> {
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<NoteDto>(
          http.get(`/v1/notes/${id}`),
        );

        if (!result.succeeded) {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<NoteDto>;
      } finally {
        this.loading = false;
      }
    },

    async createNote(payload: CreateNoteRequest): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.post("/v1/notes", payload),
        );

        if (result.succeeded) {
          // Refresh notes list after creation
          if (payload.lessonPageId) {
            await this.getNotesByLessonPage(payload.lessonPageId);
          }
        } else {
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

    async deleteNote(id: string): Promise<Result<string>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<string>(
          http.delete(`/v1/notes/${id}`),
        );

        if (result.succeeded && this.currentLessonPageId) {
          // Refresh notes list after deletion
          await this.getNotesByLessonPage(this.currentLessonPageId);
        } else if (!result.succeeded) {
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
  },
});
