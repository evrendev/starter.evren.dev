import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const useTenantStore = defineStore({
  id: "tenant",
  state: () => ({
    items: [],
    tenant: {},
    itemsLength: 0,
    loading: false
  }),
  actions: {
    async getItems({ page, itemsPerPage, sortBy, search, isActive, startDate, endDate }) {
      this.loading = true;

      try {
        const params = new URLSearchParams({
          page: page,
          itemsPerPage,
          ...(sortBy?.length && { sortBy: sortBy[0].key }),
          ...(sortBy?.length && { sortDesc: sortBy[0].order }),
          ...(isActive && { isActive }),
          ...(search && { search }),
          ...(startDate && { startDate }),
          ...(endDate && { endDate })
        });

        const response = await apiService.get(`/tenants?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        this.loading = false;
      }
    },
    async getById(id) {
      try {
        const response = await apiService.get(`/tenants/${id}`, false);
        this.tenant = response;
      } finally {
        this.loading = false;
      }
    },
    async delete(id) {
      try {
        this.loading = true;
        await apiService.delete(`/tenants/${id}`);
        this.items = this.items.filter((item) => item.id !== id);
        this.loading = false;
      } finally {
        this.loading = false;
      }
    }
  }
});
