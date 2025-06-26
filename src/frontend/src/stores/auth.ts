import { defineStore } from 'pinia'
import http from '@/utils/http'
import router from '@/router'
import { type User, type TokenResponse } from '@/models/auth'
import { type ApiResponse } from '@/models/common'

interface AuthState {
  accessToken: string | null
  refreshTokenValue: string | null
  user: User | null
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    accessToken: localStorage.getItem('accessToken'),
    refreshTokenValue: localStorage.getItem('refreshToken'),
    user: JSON.parse(localStorage.getItem('user') || 'null')
  }),
  getters: {
    isAuthenticated: (state) => !!state.accessToken,
    getUser: (state) => state.user
  },
  actions: {
    async login(email: string, password: string) {
      try {
        const response = await http.post<ApiResponse<TokenResponse>>('/auth/login', {
          email,
          password
        })
        const { data } = response.data
        this.accessToken = data.token
        this.refreshTokenValue = data.refreshToken
        this.user = data.user

        localStorage.setItem('accessToken', data.token)
        localStorage.setItem('refreshToken', data.refreshToken)
        localStorage.setItem('user', JSON.stringify(data.user))
      } catch (error) {
        console.error('Login error:', error)
        throw error
      }
    },
    async refreshToken() {
      try {
        const response = await http.get<ApiResponse<TokenResponse>>('/auth/refresh-token')
        const { data } = response.data
        this.accessToken = data.token
        this.refreshTokenValue = data.refreshToken
        this.user = data.user

        localStorage.setItem('accessToken', data.token)
        localStorage.setItem('refreshToken', data.refreshToken)
        localStorage.setItem('user', JSON.stringify(data.user))
        return true
      } catch (error) {
        console.error('Failed to refresh token:', error)
        this.logout()
        throw error
      }
    },
    async logout() {
      try {
        await http.post('/auth/logout', {})
      } catch (error) {
        console.error('Logout API call failed, but clearing local data:', error)
      } finally {
        this.accessToken = null
        this.refreshTokenValue = null
        this.user = null
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('user')
        router.push({ name: 'login' })
      }
    },
    setLanguage(lang: string) {
      localStorage.setItem('language', lang)
    },
    setTenantId(tenantId: string) {
      localStorage.setItem('tenantId', tenantId)
    }
  }
})
