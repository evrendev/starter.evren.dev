import { watchEffect } from 'vue'
import type { AxiosInstance } from 'axios'
import { useAuthStore } from '@/stores/auth'
import http from '@/utils/http'

let retryCount: number = 0
const MAX_RETRIES = 3

export function useHttpClient(): AxiosInstance {
  const authStore = useAuthStore()

  watchEffect(() => {
    http.interceptors.request.use(
      async (config) => {
        if (!config.headers['Authorization']) {
          config.headers['Authorization'] = `Bearer ${authStore.accessToken}`
        }
        return config
      },
      (error) => {
        console.error('Failed to set token', error)
      }
    )

    http.interceptors.response.use(
      (response) => response,
      async (error) => {
        const prevRequest = error?.config
        if (
          (error?.response?.status === 403 || error?.response?.status === 401) &&
          !prevRequest._retry &&
          authStore.refreshToken.length > 0
        ) {
          if (retryCount >= MAX_RETRIES) {
            window.location.href = '/auth/login'
            return Promise.reject(error)
          }

          prevRequest._retry = true
          retryCount += 1

          try {
            const result = await authStore.refresh()
            if (result.succeeded) {
              prevRequest.headers['Authorization'] = `Bearer ${authStore.accessToken}`
              retryCount = 0
              return http(prevRequest)
            } else {
              return Promise.reject(error)
            }
          } catch (error) {
            return Promise.reject(error)
          }
        }

        return Promise.reject(error)
      }
    )
  })

  return http
}
