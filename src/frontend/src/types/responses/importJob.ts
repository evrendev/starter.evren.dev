export const ImportStatus = {
  Queued: 0,
  Processing: 1,
  Completed: 2,
  Failed: 3,
} as const;

export type ImportStatusT = (typeof ImportStatus)[keyof typeof ImportStatus];

// Matches the wire format exactly: ImportLessonsFromPptxJob serializes this list with
// System.Text.Json's default (PascalCase) naming, independent of the outer ApiResponse's
// camelCase — it's stored as an opaque JSON string in ErrorsJson, not model-bound.
export interface ImportJobFailure {
  SlideIndex: number;
  Message: string;
}

export interface ImportJobDto {
  id: string;
  courseId: string;
  chapterId: string | null;
  status: ImportStatusT;
  totalSlides: number;
  processedSlides: number;
  succeededSlides: number;
  failedSlides: number;
  errorsJson: string | null;
  percentComplete: number;
  startedAt: string | null;
  completedAt: string | null;
}
