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
export interface Filters extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface AdvancedFilters {
  search: string | null;
}
