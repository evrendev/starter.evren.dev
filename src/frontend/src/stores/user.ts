import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";

export const useUserStore = defineStore("user", {
  actions: {
    async forgotPassword(email: string): Promise<string> {
      try {
        const { data } = await useHttpClient().post("/users/forgot-password", {
          email,
        });
        return data;
      } catch (error) {
        console.error("Forgot password error:", error);
        throw error;
      }
    },
  },
});
