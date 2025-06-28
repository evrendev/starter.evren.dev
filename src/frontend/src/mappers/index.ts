import type { User } from '@/models/user'
import type { ApiResponse } from '@/responses'

export default class Mapper {
  public static toUser(value: ApiResponse<User>): User {
    return {
      id: value.data.id,
      gender: value.data.gender,
      language: value.data.language,
      firstName: value.data.firstName,
      lastName: value.data.lastName,
      fullName: value.data.fullName,
      initial: value.data.initial,
      email: value.data.email,
      twoFactorEnabled: value.data.twoFactorEnabled
    }
  }
}
