import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { NotificationService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useFountainDonationStore = defineStore("fountainDonation", {
  state: () => ({
    items: [],
    donation: {},
    overview: {},
    itemsLength: 0
  }),
  actions: {
    async getItems({ page, itemsPerPage, sortBy, search, project, startDate, endDate, mediaStatus }) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: page,
          itemsPerPage,
          ...(sortBy?.length && { sortBy: sortBy[0].key }),
          ...(sortBy?.length && { sortDesc: sortBy[0].order }),
          ...(search && { search }),
          ...(project && { project }),
          ...(startDate && { startDate }),
          ...(endDate && { endDate }),
          ...(mediaStatus && { mediaStatus })
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
    async getOverviews() {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get("/donations/fountain/overview", false);
        this.overview = response;
      } finally {
        appStore.setLoading(false);
      }
    },
    async getLastDonations(message) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        await apiService.get("/donations/fountain/get-last-donations", false);
        NotificationService.success(message);
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
    async createEmptyDonation(donation) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.post("/donations/fountain/empty-donation", donation);
        this.items.pop();
        this.items.unshift(response);
      } finally {
        appStore.setLoading(false);
      }
    },
    async changeMediaInformation(mediaInformation) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/donations/fountain/media-information/${mediaInformation.id}`, mediaInformation);
        const index = this.items.findIndex((item) => item.id === mediaInformation.id);
        if (index !== -1) {
          this.items[index] = { ...response };
        }
      } finally {
        appStore.setLoading(false);
      }
    },
    async changeTeam(id, teamName) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/donations/fountain/team-name/${id}`, { id, teamName });
        const index = this.items.findIndex((item) => item.id === id);
        if (index !== -1) {
          this.items[index] = { ...response };
        }
      } finally {
        appStore.setLoading(false);
      }
    },
    async donorNotified(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/donations/fountain/donor-notified/${id}`);
        const index = this.items.findIndex((item) => item.id === id);
        if (index !== -1) {
          this.items[index] = { ...response };
        }
      } finally {
        appStore.setLoading(false);
      }
    },
    async notifyConstructionTeam(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.put(`/donations/fountain/construction-team-notified/${id}`);
        const index = this.items.findIndex((item) => item.id === id);
        if (index !== -1) {
          this.items[index] = { ...response };
        }
      } finally {
        appStore.setLoading(false);
      }
    },
    async delete(id) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const index = this.items.findIndex((item) => item.id === id);
        await apiService.delete(`/donations/fountain/${id}`);
        if (index !== -1) this.items.splice(index, 1);
      } finally {
        appStore.setLoading(false);
      }
    },
    async update(id, donation) {
      const payload = {
        ...donation,
        id
      };
      const response = await apiService.put(`/donations/fountain/${id}`, payload);
      return response;
    }
  }
});
