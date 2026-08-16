import { Chapter } from "./chapter";

export enum PageContentType {
  Text = "Text",
  Video = "Video",
  Image = "Image",
  Quiz = "Quiz",
  Embed = "Embed",
}

export interface Option {
  label: string;
  isCorrect: boolean;
  order: number;
}

export interface Question {
  prompt: string;
  order: number;
  options: Option[];
}

export interface Page {
  id: string;
  chapterId: string;
  chapterTitle?: string;
  title: string;
  content: string;
  contentType: PageContentType;
  order: number;
  mediaUrl?: string;
  // True when Content is a client-parsed pptx-to-html rich render (positioned HTML) —
  // admin editor and player both render it in a sandboxed iframe instead of Quill/v-html
  isImported?: boolean;
  // Quiz content type only — structural questions (see Task N0/N1/N2). Undefined/
  // empty means this page still uses the legacy "(richtig)" Content pattern.
  questions?: Question[];
}

export interface PageDetails extends Page {
  chapter: Chapter;
}

export interface PageProgress {
  pageId: string;
  completed: boolean;
  completedAt?: Date;
  lastVisitedAt: Date;
}
