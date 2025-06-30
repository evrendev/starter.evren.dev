export interface AccessTokenResponse {
  accessToken?: string
  refreshToken?: string
  refreshTokenExpiryTime?: Date
}

export interface UserResponse {
  id: string
  email: string
  username: string
  permissions?: string[]
}
