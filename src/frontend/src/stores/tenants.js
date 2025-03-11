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
        const params = new URLSearchParams();

        params.append("page", searchOptions?.page ?? config.page);
        params.append("itemsPerPage", searchOptions?.itemsPerPage ?? config.itemsPerPage);

        if (searchOptions?.showActiveItems != null) params.append("showActiveItems", searchOptions.showActiveItems);
        if (searchOptions?.showDeletedItems != null) params.append("showDeletedItems", searchOptions.showDeletedItems);
        if (searchOptions?.search != null) params.append("search", searchOptions.search);
        if (searchOptions?.startDate != null) params.append("startDate", searchOptions.startDate);
        if (searchOptions?.endDate != null) params.append("endDate", searchOptions.endDate);

        if (searchOptions?.sortBy?.length) {
          params.append("sortBy", searchOptions.sortBy[0]?.key);
          params.append("sortDesc", searchOptions.sortBy[0]?.order);
        }

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
