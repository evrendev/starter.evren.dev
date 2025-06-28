export interface User {
  id: string
  gender: string
  email: string
  language: string
  firstName: string
  lastName: string
  initial: string
  fullName: string
  phoneNumber?: string
  twoFactorEnabled: boolean
  imageUrl?: string
  permissions?: string[]
}
