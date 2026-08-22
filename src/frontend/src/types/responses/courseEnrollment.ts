export interface CourseEnrollmentDto {
  courseId: string;
  courseTitle: string;
  categoryTitle?: string;
  enrolledAt: string;
  pricePaid: number;
  percentComplete: number;
  chapterCount: number;
  nextChapterId: string | null;
  nextChapterTitle: string | null;
}
