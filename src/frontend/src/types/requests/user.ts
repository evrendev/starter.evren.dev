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
export interface Filters extends BasicFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface BasicFilters {
  search: string | null;
}
