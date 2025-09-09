import { PredefinedValues } from "@/models/app";
import { useHttpClient } from "@/composables/useHttpClient";
import { defineStore } from "pinia";

export const useAppStore = defineStore("app", {
  state: () => ({
    loading: false,
    predefinedValues: {} as PredefinedValues,
  }),
  getters: {
    genders: (state) => state.predefinedValues.genders || [],
    languages: (state) => state.predefinedValues.languages || [],
  },
  actions: {
    setLoading(value: boolean) {
      this.loading = value;
    },
    async getPredefinedValues() {
      this.loading = true;
      const { data } = await useHttpClient().get("/predefinedvalues");
      this.predefinedValues = data;
      this.loading = false;
    },
  },
});
