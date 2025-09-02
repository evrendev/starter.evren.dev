import { Tenant, Filters } from "@/requests/tenant";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";

const DEFAULT_FILTER: Filters = {
  search: null,
  startDate: null,
  endDate: null,
  showActiveItems: null,
  showDeletedItems: null,
  sortBy: [],
  currentPage: 1,
  itemsPerPage: 10,
};

export const useTenantStore = defineStore("tenant", {
  state: () => ({
    loading: false,
    items: [] as Tenant[],
    tenant: {} as Tenant,
    filters: {} as Filters,
  }),
  getters: {
    itemsPerPage: (state) =>
      state.filters.itemsPerPage ?? DEFAULT_FILTER.itemsPerPage,
    currentPage: (state) =>
      state.filters.currentPage ?? DEFAULT_FILTER.currentPage,
    total: (state) => state.items.length,
  },
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },
    setFilters(newFilters: Filters) {
      this.filters = { ...this.filters, ...newFilters };
    },
    async getItems(filters: Filters) {
      this.loading = true;

      try {
        const { data } = await useHttpClient().get("/tenants", {
          params: filters,
        });

        this.items = data;
      } catch (error) {
        console.error("Error fetching items:", error);
        return [];
      } finally {
        this.loading = false;
      }
    },
    async getTenant(id: string) {
      this.loading = true;

      try {
        const { data } = await useHttpClient().get(`/tenants/${id}`);

        this.tenant = data;
      } catch (error) {
        console.error("Error fetching tenant:", error);
        return null;
      } finally {
        this.loading = false;
      }
    },
  },
});
