import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import type { User } from '@/models/user'
import type { LoginRequest } from '@/requests/auth'
import type { AccessTokenResponse, UserResponse } from '@/responses/auth'
import { Result } from '@/primitives/result'
import { AppError } from '@/primitives/Error'
import { useHttpClient } from '@/composables/useHttpClient'
import type { AxiosError, AxiosResponse } from 'axios'
import Mapper from '@/mappers'

const DEFAULT_LANGUAGE = import.meta.env.VITE_APP_DEFAULT_LANGUAGE as string

const nullUser: User = {
  id: '',
  gender: 'none',
  language: DEFAULT_LANGUAGE,
  firstName: '',
  lastName: '',
  fullName: '',
  initial: '',
  twoFactorEnabled: false,
  email: '',
  permissions: []
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User>()
  const isAuthenticated = computed(() => !isLoading.value && user?.value?.id !== nullUser.id)
  const isLoading = computed(() => user?.value === undefined)
  const accessToken = ref<string>('')
  const refreshToken = ref<string>('')
  const refreshTokenExpiryTime = ref<Date | null>(null)

  function hasPermission(permission: string): boolean {
    return (isAuthenticated.value && user.value?.permissions?.includes(permission)) ?? false
  }

  async function login(email: string, password: string): Promise<Result<AccessTokenResponse>> {
    try {
      const { data } = await useHttpClient().post<
        LoginRequest,
        AxiosResponse<Result<AccessTokenResponse>>
      >('auth/login', {
        email: email,
        password: password
      })

      accessToken.value = data.data?.accessToken ?? ''
      refreshToken.value = data.data?.refreshToken ?? ''
      refreshTokenExpiryTime.value = new Date(
        data.data?.refreshTokenExpiryTime ?? Date.now() + 3600000
      )

      await getUserInfo()

      if (!data?.data) {
        return Result.failure(AppError.failure('Invalid response from server'))
      }

      return Result.success(data.data)
    } catch (error) {
      const apiError = error as AxiosError
      return Result.failure(AppError.failure(apiError.message))
    }
  }

  async function logout(): Promise<Result<string>> {
    try {
      await useHttpClient().post('auth/logout')
    } catch (error) {
      console.error('Logout failed:', error)
    }

    user.value = nullUser
    accessToken.value = ''
    refreshToken.value = ''
    refreshTokenExpiryTime.value = null

    return Result.success('Logout successful')
  }

  async function getUserInfo(): Promise<Result<string>> {
    try {
      const { data } = await useHttpClient().get<UserResponse>('personal/profile')
      const permissions = await useHttpClient().get<string[]>('personal/permissions')
      user.value = Mapper.toUser(data)
      user.value.permissions = permissions.data
    } catch (error: any) {
      user.value = nullUser
      const apiError = error as AxiosError
      return Result.failure(AppError.failure(apiError.message))
    }

    return Result.success('Logout successful')
  }

  async function refresh(): Promise<Result<string>> {
    try {
      const { data } = await useHttpClient().post<AxiosResponse<AccessTokenResponse>>(
        'auth/refresh-token',
        {
          refreshToken: refreshToken.value,
          accessToken: accessToken.value
        }
      )

      accessToken.value = data.data?.accessToken ?? ''
      refreshToken.value = data.data?.refreshToken ?? ''

      return Result.success(data.data?.accessToken ?? '')
    } catch (error) {
      const apiError = error as AxiosError
      return Result.failure(AppError.failure(apiError.message))
    }
  }

  return {
    user,
    isAuthenticated,
    login,
    logout,
    getUserInfo,
    hasPermission,
    isLoading,
    accessToken,
    refreshToken,
    refresh
  }
})
