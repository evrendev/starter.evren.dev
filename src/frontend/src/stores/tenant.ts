import { Tenant, Filters } from "@/requests/tenant";
import { DefaultApiResponse } from "@/responses/api";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { useAppStore } from "./app";
import { AxiosResponse } from "axios";
const appStore = useAppStore();

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
    tenant: {} as Tenant | null,
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
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().get(`/tenants/${id}`);

        this.tenant = data;

        return data;
      } catch (error) {
        console.error("Error fetching tenant:", error);
        return null;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async update(tenant: Tenant) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put(
          `/tenants/${tenant.id}`,
          tenant,
        );

        this.tenant = data;

        return data;
      } catch (error) {
        console.error("Error updating tenant:", error);
        return null;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async create(tenant: Tenant) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().post("/tenants", tenant);

        this.tenant = data;

        return data;
      } catch (error) {
        console.error("Error creating tenant:", error);
        return null;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async delete(id: string): Promise<DefaultApiResponse<boolean>> {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const response: AxiosResponse<DefaultApiResponse<boolean>> =
          await useHttpClient().delete(`/tenants/${id}`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items.splice(index, 1);
        }

        return response.data;
      } catch (error) {
        const response: DefaultApiResponse<boolean> = {
          succeeded: false,
          errors: [error as string],
        };

        return response;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async activate(id: string): Promise<DefaultApiResponse<boolean>> {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const response: AxiosResponse<DefaultApiResponse<boolean>> =
          await useHttpClient().post(`/tenants/${id}/activate`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items[index].isActive = true;
        }

        return response.data;
      } catch (error) {
        const response: DefaultApiResponse<boolean> = {
          succeeded: false,
          errors: [error as string],
        };

        return response;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async deactivate(id: string): Promise<DefaultApiResponse<boolean>> {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const response: AxiosResponse<DefaultApiResponse<boolean>> =
          await useHttpClient().post(`/tenants/${id}/deactivate`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items[index].isActive = false;
        }

        return response.data;
      } catch (error) {
        const response: DefaultApiResponse<boolean> = {
          succeeded: false,
          errors: [error as string],
        };

        return response;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
