import { defineStore } from "pinia";
import { useAuthStore } from "@/stores/auth";
import { apiService } from "@/utils/helpers/api";

export const useTwoFactorAuthStore = defineStore("twoFactorAuth", {
  state: () => ({
    loading: false,
    setupData: null
  }),

  getters: {
    hasError: (state) => !!state.error
  },

  actions: {
    async setup() {
      this.loading = true;

      try {
        const response = await apiService.get("/2fa/setup");
        this.setupData = response;
      } finally {
        this.loading = false;
      }
    },

    async enable(code) {
      this.loading = true;
      try {
        await apiService.post("/2fa/enable", { code });

        const authStore = useAuthStore();
        const user = authStore.user;
        user.twoFactorEnabled = true;
        authStore.updateUser(user);
      } finally {
        this.loading = false;
      }
    },

    async disable() {
      this.loading = true;
      try {
        await apiService.post("/2fa/disable");

        const authStore = useAuthStore();
        const user = authStore.user;
        user.twoFactorEnabled = false;
        authStore.updateUser(user);
      } finally {
        this.loading = false;
      }
    }
  }
});
