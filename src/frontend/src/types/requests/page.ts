import { PageContentType } from "@/models/page";

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
