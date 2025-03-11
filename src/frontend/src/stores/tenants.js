import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";
import config from "@/config";

export const useTenantStore = defineStore("tenant", {
  state: () => ({
    items: [],
    tenant: {},
    itemsLength: 0,
    loading: false,
    filters: {
      search: "",
      startDate: null,
      endDate: null,
      showActiveItems: true,
      showDeletedItems: false,
      sortBy: [],
      page: config.page,
      itemsPerPage: config.itemsPerPage
    }
  }),
  actions: {
    resetFilters() {
      this.filters = {
        search: "",
        startDate: null,
        endDate: null,
        showActiveItems: true,
        showDeletedItems: false,
        sortBy: [],
        page: config.page,
        itemsPerPage: config.itemsPerPage
      };
    },
    async getItems() {
      const appStore = useAppStore();
      this.loading = true;
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams();
        const { filters } = this; // Destructure filter state

        params.append("page", filters?.page ?? config.page);
        params.append("itemsPerPage", filters?.itemsPerPage ?? config.itemsPerPage);

        if (filters?.showActiveItems != null) params.append("showActiveItems", filters.showActiveItems);
        if (filters?.showDeletedItems != null) params.append("showDeletedItems", filters.showDeletedItems);
        if (filters?.search != null) params.append("search", filters.search);
        if (filters?.startDate != null) params.append("startDate", filters.startDate);
        if (filters?.endDate != null) params.append("endDate", filters.endDate);

        if (filters?.sortBy?.length) {
          params.append("sortBy", filters.sortBy[0]?.key);
          params.append("sortDesc", filters.sortBy[0]?.order);
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
