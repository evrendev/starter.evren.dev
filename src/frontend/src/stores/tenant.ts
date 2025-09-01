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
  itemsPerPage: 25,
};

export const useTenantStore = defineStore("tenant", {
  state: () => ({
    loading: false,
    items: [] as Tenant[],
    tenant: {} as Tenant,
    filters: {} as Filters,
  }),
  getters: {
    itemsPerPage: (state) => state.filters.itemsPerPage,
    currentPage: (state) => state.filters.currentPage,
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
  },
});
