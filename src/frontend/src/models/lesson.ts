import { Chapter } from "./chapter";

export interface Lesson {
  id: string;
  chapterId: string;
  chapterTitle?: string;
  title: string;
  content: string | undefined;
}

export interface LessonDetails extends Lesson {
  chapter: Chapter;
}
