import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const usePredefinedValuesStore = defineStore("predefined-values", {
  state: () => ({
    items: {},
    mediaStatuses: [],
    fountainTeams: [],
    loading: false
  }),
  getters: {
    genders: (state) => state.items.genders,
    languages: (state) => state.items.languages
  },
  actions: {
    async getAll() {
      this.loading = true;

      try {
        this.items = await apiService.get("/predefined-values/all", false);
      } finally {
        this.loading = false;
      }
    },
    async getMediaStatuses() {
      this.loading = true;

      try {
        this.mediaStatuses = await apiService.get("/predefined-values/media-statuses", false);
      } finally {
        this.loading = false;
      }
    },
    async getFountainTeams() {
      this.loading = true;

      try {
        this.fountainTeams = await apiService.get("/predefined-values/fountain-teams", false);
      } finally {
        this.loading = false;
      }
    }
  }
});
