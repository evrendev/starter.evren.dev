export interface AccessTokenResponse {
  accessToken?: string
  refreshToken?: string
  refreshTokenExpiryTime?: Date
}

export interface UserResponse {
  id?: string
  gender?: string
  email?: string
  language?: string
  firstName?: string
  lastName?: string
  initial?: string
  fullName?: string
  phoneNumber?: string
  twoFactorEnabled?: boolean
  imageUrl?: string
}
