export interface LoginRequest {
  email: string;
  password: string;
  response?: string | null;
  rememberMe?: boolean | null;
}
export interface TwoFactorAuthRequest {
  code: string;
  email: string;
}
