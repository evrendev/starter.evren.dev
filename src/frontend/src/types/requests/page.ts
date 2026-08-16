import { PageContentType } from "@/models/page";

export interface OptionRequest {
  label: string;
  isCorrect: boolean;
  order: number;
}

export interface QuestionRequest {
  prompt: string;
  order: number;
  options: OptionRequest[];
}

export interface CreatePageRequest {
  chapterId: string;
  title: string;
  content: string;
  contentType: PageContentType;
  order?: number;
  mediaUrl?: string;
  // Raw File from the admin's file picker (Image/Video content types) — the store
  // converts this to a base64 FileUploadRequest before sending, same pattern as
  // stores/course.ts's Course.image
  mediaFile?: File;
  // Quiz content type: structural questions, embedded replace-all (see Task N0/N1).
  // Undefined on create is equivalent to "no questions yet" — there's nothing to
  // preserve on a brand-new page.
  questions?: QuestionRequest[];
}

export interface UpdatePageRequest {
  id: string;
  chapterId: string;
  title: string;
  content: string;
  contentType?: PageContentType;
  order?: number;
  mediaUrl?: string;
  isImported?: boolean;
  mediaFile?: File;
  // undefined = leave the page's existing Questions untouched; [] = delete all of
  // them; non-empty = replace them with this list (matches backend semantics
  // exactly — see UpdatePageRequestHandler). The admin form only sets this when
  // the questions block was actually touched (see form.vue's questionsDirty).
  questions?: QuestionRequest[];
}

export interface DeletePageRequest {
  id: string;
}

export interface GetPageRequest {
  id: string;
}

export interface MarkPageCompletedRequest {
  id: string;
}

export interface PaginatePagesFilter extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}

export interface AdvancedFilters {
  search: string | null;
  chapterId: string | null;
}

export interface ExportPagesRequest {
  chapterId?: string;
}

export interface CreateNoteRequest {
  pageId: string;
  userId: string;
  content: string;
}

export interface DeleteNoteRequest {
  id: string;
}

export interface GetNoteRequest {
  id: string;
}
