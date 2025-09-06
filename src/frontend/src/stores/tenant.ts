import {
  Tenant,
  Filters,
  UpgradeTenant,
  BasicFilters,
} from "@/requests/tenant";
import { DefaultApiResponse, PaginationResponse } from "@/responses/api";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { useAppStore } from "./app";
import { AxiosError, AxiosResponse } from "axios";
const appStore = useAppStore();

const DEFAULT_FILTER: Filters = {
  search: null,
  startDate: null,
  endDate: null,
  showActiveItems: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const useTenantStore = defineStore("tenant", {
  state: () => ({
    loading: false as boolean,
    currentPage: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    totalCount: 0 as number,
    pageSize: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,
    items: [] as Tenant[],
    tenant: null as Tenant | null,
    filters: DEFAULT_FILTER,
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },
    setFilters(basicFilters: BasicFilters) {
      this.filters = { ...this.filters, ...basicFilters };
    },
    async getItems() {
      this.loading = true;

      try {
        const { data }: AxiosResponse<PaginationResponse<Tenant>> =
          await useHttpClient().get("/tenants", {
            params: this.filters,
          });

        this.items = data.data;
        this.currentPage = data.currentPage;
        this.totalPages = data.totalPages;
        this.totalCount = data.totalCount;
        this.pageSize = data.pageSize;
        this.hasNextPage = data.hasNextPage;
        this.hasPreviousPage = data.hasPreviousPage;
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
    async upgrade(tenant: UpgradeTenant): Promise<DefaultApiResponse<boolean>> {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const response: AxiosResponse<DefaultApiResponse<boolean>> =
          await useHttpClient().post(
            `/tenants/${tenant.tenantId}/upgrade`,
            tenant,
          );

        if (response.data.succeeded) {
          const index = this.items.findIndex(
            (item) => item.id === tenant.tenantId,
          );

          if (index !== -1)
            this.items[index].validUpto = tenant.extendedExpiryDate;
        }

        return response.data;
      } catch (error: unknown) {
        const axiosError = error as AxiosError<any, any>;
        const response: DefaultApiResponse<boolean> = {
          succeeded: false,
          errors: [axiosError.response?.data?.message || axiosError.message],
        };

        return response;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
