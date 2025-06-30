import type { User } from '@/models/user'
import type { UserResponse } from '@/responses/auth'

export default class Mapper {
  public static toUser(value: UserResponse): User {
    return {
      id: value.id,
      gender: value.gender,
      language: value.language,
      firstName: value.firstName,
      lastName: value.lastName,
      fullName: value.fullName,
      initial: value.initial,
      email: value.email,
      twoFactorEnabled: value.twoFactorEnabled
    }
  }
}
