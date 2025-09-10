import { BasicUser, User } from "@/models/user";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { useAppStore } from "./app";
import Mapper from "@/mappers";
import { DefaultApiResponse } from "@/responses/api";

export const usePersonalStore = defineStore("personal", {
  state: () => ({
    loading: false as boolean,
    user: null as BasicUser | null,
    permissions: [] as string[],
  }),
  actions: {
    async getUser() {
      this.loading = true;
      const appStore = useAppStore();
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

    async getPermissions() {
      this.loading = true;
      try {
        const { data } = await useHttpClient().get<string[]>(
          "personal/permissions",
        );
        this.permissions = data || [];
      } catch (error: unknown) {
        console.error("Failed to fetch permissions:", error);
        this.permissions = [];
      } finally {
        this.loading = false;
      }
    },

    hasPermission(permission: string | string[]): boolean {
      if (!this.permissions || this.permissions.length === 0) {
        return false;
      }
      if (Array.isArray(permission)) {
        return permission.every((perm) => this.permissions.includes(perm));
      }
      return this.permissions.includes(permission);
    },

    async update(user: BasicUser) {
      this.loading = true;
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put<DefaultApiResponse<User>>(
          "personal/profile",
          user,
        );

        this.user = Mapper.toUser(data.data);
        return data;
      } catch (error: unknown) {
        console.error("Failed to update profile:", error);
        throw error;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    clearProfile() {
      this.user = {} as BasicUser;
      this.permissions = [];
    },
  },
});
