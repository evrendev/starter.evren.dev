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
