import { defineStore } from "pinia";
import { useAuthStore } from "@/stores/auth";
import { apiService } from "@/utils/helpers";
import LocaleHelper from "@/utils/helpers/locale";

export const useUserStore = defineStore({
  id: "user",
  state: () => ({
    items: [],
    user: {},
    itemsLength: 0,
    loading: false,
    reset: false
  }),
  actions: {
    async getItems({ page, itemsPerPage, sortBy, search, action, startDate, endDate }) {
      this.loading = true;

      try {
        const params = new URLSearchParams({
          page: page,
          itemsPerPage,
          ...(sortBy?.length && { sortBy: sortBy[0].key }),
          ...(sortBy?.length && { sortDesc: sortBy[0].order }),
          ...(search && { search }),
          ...(action && { action }),
          ...(startDate && { startDate }),
          ...(endDate && { endDate })
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
