export interface CourseEnrollmentDto {
  courseId: string;
  courseTitle: string;
  categoryTitle?: string;
  enrolledAt: string;
  pricePaid: number;
  percentComplete: number;
}
