import { defineStore } from "pinia";
import { useAuthStore } from "@/stores/auth";
import { apiService } from "@/utils/helpers/api";
import { useAppStore } from "@/stores";

export const useTwoFactorAuthStore = defineStore("twoFactorAuth", {
  state: () => ({
    setupData: null
  }),

  getters: {
    hasError: (state) => !!state.error
  },

  actions: {
    async setup() {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get("/2fa/setup");
        this.setupData = response;
      } finally {
        appStore.setLoading(false);
      }
    },

    async enable(code) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        await apiService.post("/2fa/enable", { code });

        const authStore = useAuthStore();
        const user = authStore.user;
        user.twoFactorEnabled = true;
        authStore.updateUser(user);
      } finally {
        appStore.setLoading(false);
      }
    },

    async disable() {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        await apiService.post("/2fa/disable");

        const authStore = useAuthStore();
        const user = authStore.user;
        user.twoFactorEnabled = false;
        authStore.updateUser(user);
      } finally {
        appStore.setLoading(false);
      }
    }
  }
});
