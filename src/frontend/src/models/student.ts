export interface StudentSummary {
  userId: string;
  fullName?: string;
  email?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  enrolledCourseCount: number;
  totalPaid: number;
  averageCompletionPercent: number;
}

export interface StudentsSummaryStats {
  totalRevenue: number;
  averageCompletionPercent: number;
}

export interface StudentEnrollment {
  courseId: string;
  courseTitle: string;
  enrolledAt: string;
  pricePaid: number;
  percentComplete: number;
}

export interface StudentPayment {
  courseId: string;
  courseTitle: string;
  amount: number;
  currency: string;
  status: string;
  payPalCaptureId?: string | null;
  capturedAt?: string | null;
}

export interface StudentDetail {
  userId: string;
  fullName?: string;
  email?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  enrollments: StudentEnrollment[];
  payments: StudentPayment[];
}
