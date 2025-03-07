import { defineStore } from "pinia";
import { useAuthStore } from "@/stores/auth";
import { apiService } from "@/utils/helpers";
import LocaleHelper from "@/utils/helpers/locale";
import { useAppStore } from "@/stores";

export const useUserStore = defineStore("user", {
  state: () => ({
    items: [],
    user: {},
    itemsLength: 0,
    reset: false
  }),
  getters: {
    userInformations: (state) =>
      new Object({
        id: state.user.id,
        tenantId: state.user.tenantId,
        gender: state.user.gender,
        initial: state.user.initial,
        email: state.user.email,
        firstName: state.user.firstName,
        lastName: state.user.lastName,
        fullName: state.user.fullName,
        jobTitle: state.user.jobTitle,
        language: state.user.language,
        twoFactorEnabled: state.user.twoFactorEnabled
      }),
    userPermissions: (state) => state.user.permissions
  },
  actions: {
    async getItems(options) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: options.page,
          itemsPerPage: options.itemsPerPage,
          ...(options.sortBy?.length && { sortBy: options.sortBy[0].key }),
          ...(options.sortBy?.length && { sortDesc: options.sortBy[0].order }),
          ...(options.search && { search: options.search }),
          ...(options.action && { action: options.action }),
          ...(options.startDate && { startDate: options.startDate }),
          ...(options.endDate && { endDate: options.endDate }),
          ...(options.showDeletedItems && { showDeletedItems: options.showDeletedItems })
        });

        const response = await apiService.get(`/users?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        appStore.setLoading(false);
      }
    },
    async getById(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get(`/users/${id}`, false);
        this.user = response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async delete(id) {
      this.reset = true;

      try {
        await apiService.delete(`/users/${id}`);
      } finally {
        this.reset = false;
      }
    },
    async restore(id) {
      this.reset = true;

      try {
        await apiService.post(`/users/${id}/restore`);
      } finally {
        this.reset = false;
      }
    },
    async create(user) {
      // No need to manage loading state here as apiService.post already handles it
      const response = await apiService.post("/users", user);
      return response;
    },
    async update(id, user) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/users/${id}`, user);
        const authStore = useAuthStore();
        authStore.updateUser(user);

        await LocaleHelper.switchLanguage(user.language);
        return response;
      } finally {
        appStore.setLoading(false);
      }
    }
  }
});
