import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const useTenantStore = defineStore({
  id: "tenant",
  state: () => ({
    items: [],
    tenant: {},
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
          itemsPerPage: searchOptions.itemsPerPage,
          ...(searchOptions.isActive !== undefined && searchOptions.isActive !== null && { isActive: searchOptions.isActive }),
          ...(searchOptions.sortBy?.length && { sortBy: searchOptions.sortBy[0].key }),
          ...(searchOptions.sortBy?.length && { sortDesc: searchOptions.sortBy[0].order }),
          ...(searchOptions.search !== undefined && searchOptions.search !== null && { search: searchOptions.search }),
          ...(searchOptions.startDate !== undefined && searchOptions.startDate !== null && { startDate: searchOptions.startDate }),
          ...(searchOptions.endDate !== undefined && searchOptions.endDate !== null && { endDate: searchOptions.endDate })
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
        return response;
      } finally {
        this.loading = false;
      }
    },
    async delete(id) {
      try {
        this.loading = true;
        this.reset = true;

        await apiService.delete(`/tenants/${id}`);
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

        await apiService.post(`/tenants/${id}/restore`);
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

        await apiService.post(`/tenants/${id}/activate`);
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

        await apiService.post(`/tenants/${id}/deactivate`);
        this.loading = false;
      } finally {
        this.loading = false;
        this.reset = false;
      }
    },
    async create(tenant) {
      try {
        this.loading = true;
        const response = await apiService.post("/tenants", tenant);
        return response;
      } finally {
        this.loading = false;
      }
    },
    async update(id, tenant) {
      try {
        this.loading = true;
        const response = await apiService.put(`/tenants/${id}`, tenant);
        return response;
      } finally {
        this.loading = false;
      }
    }
  }
});
