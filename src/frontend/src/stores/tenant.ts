import { defineStore } from "pinia";
import { useAppStore } from "./app";

import { Tenant } from "@/models/tenant";
import { Filters, UpgradeTenant, BasicFilters } from "@/types/requests/tenant";
import { PaginationResponse } from "@/types/responses/api";

import http, { handleRequest } from "@/utils/http";
import { AppError } from "@/primitives/error";
import { Result } from "@/primitives/result";

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

    error: null as AppError | null,

    page: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    total: 0 as number,
    itemsPerPage: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,

    items: [] as Tenant[],
    tenant: null as Tenant | null,
    filters: { ...DEFAULT_FILTER },
  }),
  actions: {
    resetFilters() {
      this.filters = { ...DEFAULT_FILTER };
    },
    setFilters(filters: BasicFilters) {
      this.filters = { ...this.filters, ...filters };
    },

    async getItems() {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<PaginationResponse<Tenant>>(
          http.get("/tenants", {
            params: this.filters,
          }),
        );

        if (result.succeeded && result.data) {
          this.items = result.data.items;
          this.page = result.data.page;
          this.total = result.data.total;
          this.itemsPerPage = result.data.itemsPerPage;
          this.totalPages = result.data.totalPages;
          this.hasNextPage = result.data.hasNextPage;
          this.hasPreviousPage = result.data.hasPreviousPage;
        } else {
          this.error = result.errors!;
          this.items = [];
        }

        this.loading = false;
      } catch (error) {
        this.error = error as AppError;
        this.items = [];
        return error as Result<PaginationResponse<Tenant>>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async getById(id: string): Promise<Result<Tenant>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Tenant>(http.get(`/tenants/${id}`));

        if (result.succeeded && result.data) {
          this.tenant = result.data;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        this.tenant = null;
        return error as Result<Tenant>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async update(tenant: Tenant): Promise<Result<Tenant>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Tenant>(
          http.put(`/tenants/${tenant.id}`, tenant),
        );

        if (result.succeeded && result.data) {
          this.tenant = result.data;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Tenant>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async create(tenant: Tenant): Promise<Result<Tenant>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<Tenant>(
          http.post("/tenants", tenant),
        );

        if (result.succeeded && result.data) {
          this.tenant = result.data;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<Tenant>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async delete(id: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.delete(`/tenants/${id}`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items.splice(index, 1);
        } else {
          this.error = result.errors!;
        }

        this.loading = false;
        appStore.setLoading(false);
        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async activate(id: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/tenants/${id}/activate`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items[index].isActive = true;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async deactivate(id: string): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/tenants/${id}/deactivate`),
        );

        if (result.succeeded) {
          const index = this.items.findIndex((item) => item.id === id);
          if (index !== -1) this.items[index].isActive = false;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },

    async upgrade(tenant: UpgradeTenant): Promise<Result<boolean>> {
      const appStore = useAppStore();
      appStore.setLoading(true);
      this.loading = true;
      this.error = null;

      try {
        const result = await handleRequest<boolean>(
          http.post(`/tenants/${tenant.tenantId}/upgrade`, tenant),
        );

        if (result.succeeded) {
          const index = this.items.findIndex(
            (item) => item.id === tenant.tenantId,
          );
          if (index !== -1)
            this.items[index].validUpto = tenant.extendedExpiryDate;
        } else {
          this.error = result.errors!;
        }

        return result;
      } catch (error) {
        this.error = error as AppError;
        return error as Result<boolean>;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
  },
});
