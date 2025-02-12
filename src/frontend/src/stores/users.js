import { defineStore } from "pinia";
import { useAuthStore } from "@/stores/auth";
import { apiService } from "@/utils/helpers";
import LocaleHelper from "@/utils/helpers/locale";

export const useUserStore = defineStore("user", {
  state: () => ({
    items: [],
    user: {},
    itemsLength: 0,
    loading: false,
    reset: false
  }),
  actions: {
    async getItems(options) {
      this.loading = true;
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
        this.loading = false;
      }
    },
    async getById(id) {
      try {
        const response = await apiService.get(`/users/${id}`, false);
        this.user = response;
      } finally {
        this.loading = false;
      }
    },
    async delete(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.delete(`/users/${id}`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async restore(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.post(`/users/${id}/restore`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async create(user) {
      try {
        this.loading = true;
        const response = await apiService.post("/users", user);
        return response;
      } finally {
        this.loading = false;
      }
    },
    async update(id, user) {
      try {
        this.loading = true;
        const response = await apiService.put(`/users/${id}`, user);
        const authStore = useAuthStore();
        authStore.updateUser(user);

        await LocaleHelper.switchLanguage(user.language);
        return response;
      } finally {
        this.loading = false;
      }
    }
  }
});
