import { LessonPageContentType } from "@/models/lessonPage";

export interface CreateLessonPageRequest {
  lessonId: string;
  title: string;
  content: string;
  contentType: LessonPageContentType;
  order?: number;
  mediaUrl?: string;
}

export interface UpdateLessonPageRequest {
  id: string;
  lessonId: string;
  title: string;
  content: string;
  contentType?: LessonPageContentType;
  order?: number;
  mediaUrl?: string;
}

export interface DeleteLessonPageRequest {
  id: string;
}

export interface GetLessonPageRequest {
  id: string;
}

export interface MarkLessonPageCompletedRequest {
  id: string;
}

export interface PaginateLessonPagesFilter extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}

export interface AdvancedFilters {
  search: string | null;
  lessonId: string | null;
}

export interface ExportLessonPagesRequest {
  lessonId?: string;
}
