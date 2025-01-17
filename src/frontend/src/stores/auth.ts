import { defineStore } from 'pinia';
import { router } from '@/router';
import axiosInstance from '@/plugins/axios';

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuthStore = defineStore({
  id: 'auth',
  state: () => ({
    // initialize state from local storage to enable user to stay logged in
    /* eslint-disable-next-line @typescript-eslint/ban-ts-comment */
    // @ts-ignore
    user: null,
    userId: null,
    token: null,
    refreshToken: null,
    returnUrl: null,
    rememberMe: false
  }),
  actions: {
    async login(email: string, password: string, rememberMe: boolean, response: string) {
      const user = await axiosInstance.post(`${baseUrl}/auth/login`, { email, password, rememberMe, response });

      if (user?.data?.requiresTwoFactor) {
        this.userId = user?.data?.userId;
        this.rememberMe = rememberMe;

        router.push(`/auth/login/2fa`);
      } else {
        // update pinia state
        this.user = user?.data.user;
        this.token = user?.data.token;
        this.refreshToken = user?.data.refreshToken;

        router.push(this.returnUrl || '/dashboard');
      }
    },
    async verify(code: string) {
      const rememberMachine = this.rememberMe;
      const userId = this.userId;
      const user = await axiosInstance.post(`${baseUrl}/2fa/verify`, { code, userId, rememberMachine });

      // update pinia state
      this.user = user?.data.user;
      this.token = user?.data.token;
      this.refreshToken = user?.data.refreshToken;

      router.push(this.returnUrl || '/dashboard');
    },
    logout() {
      this.user = null;
      this.token = null;
      this.refreshToken = null;
      this.userId = null;
      this.rememberMe = false;

      localStorage.removeItem('user');
      router.push('/login');
    }
  }
});
