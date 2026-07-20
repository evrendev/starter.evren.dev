import { PageContentType } from "@/models/page";
import { Chapter } from "@/models/chapter";

export interface PageDto {
  id: string;
  title: string;
  chapterId: string;
  chapterTitle?: string;
  order: number;
  contentType?: string;
  needsReview?: boolean;
  isImported?: boolean;
}

export interface PageDetailsDto extends PageDto {
  content?: string;
  mediaUrl?: string;
  chapter?: Chapter;
}

export interface PageExportDto {
  id: string;
  title: string;
  chapterTitle: string;
  order: number;
}

export interface ChapterPlayerPageDto {
  id: string;
  title: string;
  order: number;
  contentType: string;
  content?: string;
  mediaUrl?: string;
  isImported?: boolean;
  completed?: boolean;
  completedAt?: Date;
}

export interface ChapterPlayerDto {
  chapterId: string;
  chapterTitle: string;
  pages: ChapterPlayerPageDto[];
  percentComplete?: number;
  lastVisitedPageId?: string;
}

export interface NoteDto {
  id: string;
  userId: string;
  pageId: string;
  content: string;
  // Backend maps this from the entity's LastModifiedOn (set at create time)
  createdOn?: string;
}
