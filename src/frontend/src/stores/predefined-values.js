import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const usePredefinedValuesStore = defineStore({
  id: "predefinedValues",
  state: () => ({
    items: {},
    loading: false
  }),
  getters: {
    genders: (state) => state.items.genders,
    languages: (state) => state.items.languages
  },
  actions: {
    async get() {
      this.loading = true;

      try {
        this.items = await apiService.get("/predefined-values", false);
      } finally {
        this.loading = false;
      }
    }
  }
});
