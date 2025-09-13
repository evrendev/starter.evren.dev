import { PredefinedValues } from "@/models/app";
import http, { handleRequest } from "@/utils/http";
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
      const result = await handleRequest<PredefinedValues>(
        http.get("predefinedvalues"),
      );

      this.predefinedValues = result?.data || { genders: [], languages: [] };
      this.loading = false;
    },
  },
});
