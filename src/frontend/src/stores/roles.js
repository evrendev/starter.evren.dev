import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useRoleStore = defineStore("role", {
  state: () => ({
    items: [],
    role: {},
    itemsLength: 0,
    reset: false
  }),
  actions: {
    async getItems(searchOptions) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: searchOptions.page,
          itemsPerPage: searchOptions.itemsPerPage
        });

        const response = await apiService.get(`/roles?${params}`, false);
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
        const response = await apiService.get(`/roles/${id}`, false);
        this.role = response;
        return response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async delete(id) {
      this.reset = true;

      try {
        await apiService.delete(`/roles/${id}`);
      } finally {
        this.reset = false;
      }
    },
    async restore(id) {
      this.reset = true;

      try {
        await apiService.post(`/roles/${id}/restore`);
      } finally {
        this.reset = false;
      }
    },
    async activate(id) {
      this.reset = true;

      try {
        await apiService.post(`/roles/${id}/activate`);
      } finally {
        this.reset = false;
      }
    },
    async deactivate(id) {
      this.reset = true;

      try {
        await apiService.post(`/roles/${id}/deactivate`);
      } finally {
        this.reset = false;
      }
    },
    async create(role) {
      const response = await apiService.post("/roles", role);
      return response;
    },
    async update(id, role) {
      const response = await apiService.put(`/roles/${id}`, role);
      return response;
    }
  }
});
