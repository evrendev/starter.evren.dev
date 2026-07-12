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
