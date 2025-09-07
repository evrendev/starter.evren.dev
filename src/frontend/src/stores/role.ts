import { Role, Filters, BasicFilters } from "@/requests/role";
import { DefaultApiResponse, PaginationResponse } from "@/responses/api";
import { defineStore } from "pinia";
import { useHttpClient } from "@/composables/useHttpClient";
import { useAppStore } from "./app";
import { AxiosResponse } from "axios";
const appStore = useAppStore();

const DEFAULT_FILTER: Filters = {
  search: null,
  showActiveItems: null,
  sortBy: [],
  groupBy: [],
  page: 1,
  itemsPerPage: 25,
};

export const useRoleStore = defineStore("role", {
  state: () => ({
    loading: false as boolean,
    page: DEFAULT_FILTER.page as number,
    totalPages: 0 as number,
    total: 0 as number,
    itemsPerPage: DEFAULT_FILTER.itemsPerPage as number,
    hasNextPage: false as boolean,
    hasPreviousPage: false as boolean,
    items: [] as Role[],
    role: null as Role | null,
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
        const { data }: AxiosResponse<PaginationResponse<Role>> =
          await useHttpClient().get("/roles", {
            params: this.filters,
          });

        this.items = data.items;
        this.page = data.page;
        this.total = data.total;
        this.itemsPerPage = data.itemsPerPage;
        this.totalPages = data.totalPages;
        this.hasNextPage = data.hasNextPage;
        this.hasPreviousPage = data.hasPreviousPage;
      } catch (error) {
        console.error("Error fetching items:", error);
        return [];
      } finally {
        this.loading = false;
      }
    },
    async getRole(id: string) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().get(`/roles/${id}`);

        this.role = data;

        return data;
      } catch (error) {
        console.error("Error fetching role:", error);
        return null;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async update(role: Role) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().put(`/roles/${role.id}`, role);

        this.role = data;

        return data;
      } catch (error) {
        console.error("Error updating role:", error);
        return null;
      } finally {
        this.loading = false;
        appStore.setLoading(false);
      }
    },
    async create(role: Role) {
      this.loading = true;
      appStore.setLoading(true);

      try {
        const { data } = await useHttpClient().post("/roles", role);

        this.role = data;

        return data;
      } catch (error) {
        console.error("Error creating role:", error);
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
          await useHttpClient().delete(`/roles/${id}`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item: Role) => item.id === id);
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
          await useHttpClient().post(`/roles/${id}/activate`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item: Role) => item.id === id);
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
          await useHttpClient().post(`/roles/${id}/deactivate`);

        if (response.data.succeeded) {
          const index = this.items.findIndex((item: Role) => item.id === id);
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
