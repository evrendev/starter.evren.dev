import type { User } from '@/models/user'
import type { Result } from '@/primitives/Result'

export default class Mapper {
  public static toUser(value: Result<User>): User {
    return {
      id: value.data?.id,
      gender: value.data?.gender,
      language: value.data?.language,
      firstName: value.data?.firstName,
      lastName: value.data?.lastName,
      fullName: value.data?.fullName,
      initial: value.data?.initial,
      email: value.data?.email,
      twoFactorEnabled: value.data?.twoFactorEnabled
    }
  }
}
