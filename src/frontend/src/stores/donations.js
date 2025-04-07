import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useDonationStore = defineStore("donation", {
  state: () => ({
    items: [],
    donation: {},
    itemsLength: 0
  }),
  actions: {
    async getItems({ page, itemsPerPage, sortBy, search, projectCode, startDate, endDate }) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: page,
          itemsPerPage,
          ...(sortBy?.length && { sortBy: sortBy[0].key }),
          ...(sortBy?.length && { sortDesc: sortBy[0].order }),
          ...(search && { search }),
          ...(projectCode && { projectCode }),
          ...(startDate && { startDate }),
          ...(endDate && { endDate })
        });

        const response = await apiService.get(`/donations?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        appStore.setLoading(false);
      }
    },
    async getById(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get(`/donations/${id}`, false);
        this.donation = response;
      } finally {
        appStore.setLoading(false);
      }
    }
  }
});
