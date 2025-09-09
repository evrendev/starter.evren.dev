import { User } from "@/models/user";

export interface LoginRequest {
  email: string;
  password: string;
  response?: string | null;
  rememberMe?: boolean | null;
}
