import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const useRoleStore = defineStore({
  id: "role",
  state: () => ({
    items: [],
    role: {},
    itemsLength: 0,
    loading: false,
    reset: false
  }),
  actions: {
    async getItems(searchOptions) {
      this.loading = true;

      try {
        const params = new URLSearchParams({
          page: searchOptions.page,
          itemsPerPage: searchOptions.itemsPerPage
        });

        const response = await apiService.get(`/roles?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        this.loading = false;
      }
    },
    async getById(id) {
      try {
        const response = await apiService.get(`/roles/${id}`, false);
        this.role = response;
        return response;
      } finally {
        this.loading = false;
      }
    },
    async delete(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.delete(`/roles/${id}`);
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

        await apiService.post(`/roles/${id}/restore`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async activate(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.post(`/roles/${id}/activate`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async deactivate(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.post(`/roles/${id}/deactivate`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async create(role) {
      try {
        this.loading = true;
        const response = await apiService.post("/roles", role);
        return response;
      } finally {
        this.loading = false;
      }
    },
    async update(id, role) {
      try {
        this.loading = true;
        const response = await apiService.put(`/roles/${id}`, role);
        return response;
      } finally {
        this.loading = false;
      }
    }
  }
});
