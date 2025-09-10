export interface ChangePasswordRequest {
  password: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
  response?: string | null;
}

export interface ResetPasswordRequest {
  email?: string | null;
  password?: string | null;
  confirmPassword?: string | null;
  token?: string | null;
}

export interface LogFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
  startDate: string | null;
  endDate: string | null;
  search: string | null;
}

export interface ChangePasswordRequest {
  password: string;
  newPassword: string;
  confirmNewPassword: string;
}
