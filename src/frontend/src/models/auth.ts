export interface TokenRequest {
  email: string
  password: string
  response?: string | null
  rememberMe?: boolean | null
}

export interface TokenResponse {
  refreshToken: (() => Promise<boolean>) & string
  token: string
  refreshTokenExpiryTime: string
  user: User | null
}

export interface User {
  id: string | null
  gender: string | null
  language: string | null
  firstName: string | null
  lastName: string | null
  initial: string
  email: string | null
  fullName: string | null
  twoFactorEnabled: boolean
}

export interface ChangePasswordRequest {
  password: string
  newPassword: string
  confirmNewPassword: string
}

export interface ForgotPasswordRequest {
  email: string
}

export interface ResetPasswordRequest {
  email?: string | null
  password?: string | null
  token?: string | null
}
