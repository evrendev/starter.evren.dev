export interface LoginRequest {
  email: string;
  password: string;
  response?: string | null;
  rememberMe?: boolean | null;
}
