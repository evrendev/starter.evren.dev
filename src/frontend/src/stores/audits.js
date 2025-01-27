import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const useAuditStore = defineStore({
  id: "audit",
  state: () => ({
    items: [],
    itemsLength: 0,
    loading: false
  }),
  actions: {
    async load({ page, itemsPerPage, sortBy, search }) {
      this.loading = true;

      try {
        const params = new URLSearchParams({
          page: page,
          itemsPerPage,
          ...(sortBy?.length && { sortBy: sortBy[0].key }),
          ...(sortBy?.length && { sortDesc: sortBy[0].order }),
          search
        });

        const response = await apiService.get(`/audits?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        this.loading = false;
      }
    }
  }
});
