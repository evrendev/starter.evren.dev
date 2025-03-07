import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";
import config from "@/config";

export const useTenantStore = defineStore("tenant", {
  state: () => ({
    items: [],
    tenant: {},
    itemsLength: 0,
    loading: false
  }),
  actions: {
    async getItems(searchOptions) {
      const appStore = useAppStore();
      this.loading = true;
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: searchOptions?.page ?? 1,
          itemsPerPage: searchOptions?.itemsPerPage ?? config.itemsPerPage,
          ...(searchOptions?.isActive !== undefined && searchOptions?.isActive !== null && { isActive: searchOptions?.isActive }),
          ...(searchOptions?.showDeletedItems !== undefined &&
            searchOptions?.showDeletedItems !== null && { showDeletedItems: searchOptions?.showDeletedItems }),
          ...(searchOptions?.sortBy?.length && { sortBy: searchOptions?.sortBy[0]?.key }),
          ...(searchOptions?.sortBy?.length && { sortDesc: searchOptions?.sortBy[0]?.order }),
          ...(searchOptions?.search !== undefined && searchOptions?.search !== null && { search: searchOptions?.search }),
          ...(searchOptions?.startDate !== undefined && searchOptions?.startDate !== null && { startDate: searchOptions?.startDate }),
          ...(searchOptions?.endDate !== undefined && searchOptions?.endDate !== null && { endDate: searchOptions?.endDate })
        });

        const response = await apiService.get(`/tenants?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        appStore.setLoading(false);
        this.loading = false;
      }
    },
    async getById(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get(`/tenants/${id}`, false);
        this.tenant = response;
        return response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async delete(id) {
      try {
        await apiService.delete(`/tenants/${id}`);
      } finally {
        await this.getItems();
      }
    },
    async restore(id) {
      try {
        await apiService.post(`/tenants/${id}/restore`);
      } finally {
        await this.getItems();
      }
    },
    async activate(id) {
      try {
        await apiService.post(`/tenants/${id}/activate`);
      } finally {
        await this.getItems();
      }
    },
    async deactivate(id) {
      try {
        await apiService.post(`/tenants/${id}/deactivate`);
      } finally {
        await this.getItems();
      }
    },
    async create(tenant) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.post("/tenants", tenant);
        this.tenant = response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async update(id, tenant) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/tenants/${id}`, tenant);
        this.tenant = response;
      } finally {
        appStore.setLoading(false);
      }
    }
  }
});
