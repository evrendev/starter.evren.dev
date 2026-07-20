import { Chapter } from "./chapter";

export enum PageContentType {
  Text = "Text",
  Video = "Video",
  Image = "Image",
  Quiz = "Quiz",
  Embed = "Embed",
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
