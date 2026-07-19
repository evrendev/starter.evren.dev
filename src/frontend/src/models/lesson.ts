import { Chapter } from "./chapter";

export interface Lesson {
  id: string;
  chapterId: string;
  chapterTitle?: string;
  title: string;
  content: string | undefined;
  isStaging?: boolean;
  needsReview?: boolean;
}

export interface LessonDetails extends Lesson {
  chapter: Chapter;
}
