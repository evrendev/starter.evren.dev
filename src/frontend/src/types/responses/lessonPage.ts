import { LessonPageContentType } from "@/models/lessonPage";
import { Lesson } from "@/models/lesson";

export interface LessonPageDto {
  id: string;
  title: string;
  lessonId: string;
  lessonTitle?: string;
  order: number;
  contentType?: string;
}

export interface LessonPageDetailsDto extends LessonPageDto {
  content?: string;
  contentType?: string;
  mediaUrl?: string;
  isImported?: boolean;
  lesson?: Lesson;
}

export interface LessonPageExportDto {
  id: string;
  title: string;
  lessonTitle: string;
  order: number;
}

export interface LessonPlayerPageDto {
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

export interface LessonPlayerDto {
  lessonId: string;
  lessonTitle: string;
  pages: LessonPlayerPageDto[];
  percentComplete?: number;
  lastVisitedPageId?: string;
}

export interface NoteDto {
  id: string;
  userId: string;
  lessonPageId: string;
  content: string;
  // Backend maps this from the entity's LastModifiedOn (set at create time)
  createdOn?: string;
}
