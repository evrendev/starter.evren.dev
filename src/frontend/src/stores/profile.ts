import { BasicUser, User } from "@/models/user";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { useAppStore } from "./app";

const appStore = useAppStore();
import Mapper from "@/mappers";

export const useProfileStore = defineStore("profile", {
  state: () => ({
    loading: false as boolean,
    user: {} as BasicUser,
  }),
  actions: {
    async get() {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().get<User>("personal/profile");
        this.user = Mapper.toUser(data);
      } catch (error: unknown) {
        console.error("Failed to fetch profile:", error);
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async update(user: BasicUser) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put<User>(
          "personal/profile",
          user,
        );
        console.log("Profile updated:", data);
        this.user = user;
      } catch (error: unknown) {
        console.error("Failed to update profile:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
