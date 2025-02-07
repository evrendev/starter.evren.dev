import { defineStore } from "pinia";
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
      } finally {
        this.loading = false;
      }
    },

    async disable() {
      this.loading = true;
      try {
        await apiService.post("/2fa/disable");
      } finally {
        this.loading = false;
      }
    }
  }
});
