import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { ResetPasswordRequest } from "@/types/requests/user";

export const useUserStore = defineStore("user", {
  actions: {
    async forgotPassword(email: string): Promise<string> {
      try {
        const { data } = await http.post("/users/forgot-password", {
          email,
        });
        return data;
      } catch (error) {
        console.error("Forgot password error:", error);
        throw error;
      }
    },
    async resetPassword(data: ResetPasswordRequest): Promise<string> {
      try {
        const { data: response } = await http.post(
          "/users/reset-password",
          data,
        );
        return response;
      } catch (error) {
        console.error("Reset password error:", error);
        throw error;
      }
    },
  },
});
