import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useFountainDonationStore = defineStore("fountainDonation", {
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

        const response = await apiService.get(`/donations/fountain?${params}`, false);
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
        const response = await apiService.get(`/donations/fountain/${id}`, false);
        this.donation = response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async create(donation) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.post("/donations/fountain", donation);
        this.donation = response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async changeMediaStatus(id, mediastatus) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/donations/fountain/media-status/${id}`, { id, mediastatus });
        this.donation = response;
      } finally {
        appStore.setLoading(false);
      }
    }
  }
});
