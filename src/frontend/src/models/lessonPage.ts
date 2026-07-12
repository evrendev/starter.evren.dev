import { Lesson } from "./lesson";

export enum LessonPageContentType {
  Text = "Text",
  Video = "Video",
  Image = "Image",
  Quiz = "Quiz",
  Embed = "Embed",
}

export interface LessonPage {
  id: string;
  lessonId: string;
  lessonTitle?: string;
  title: string;
  content: string;
  contentType: LessonPageContentType;
  order: number;
  mediaUrl?: string;
}

export interface LessonPageDetails extends LessonPage {
  lesson: Lesson;
}

export interface LessonPageProgress {
  lessonPageId: string;
  completed: boolean;
  completedAt?: Date;
  lastVisitedAt: Date;
}
