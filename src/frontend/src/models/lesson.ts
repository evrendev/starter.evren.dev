import { Chapter } from "./chapter";

export interface Lesson {
  id: string;
  chapterId: string;
  chapterTitle?: string;
  title: string;
  description: string | null;
  content: string | undefined;
  notes: string | null;
  image: File | File[] | undefined;
}

export interface LessonDetails extends Lesson {
  chapter: Chapter;
  imageUrl?: string | null;
}
