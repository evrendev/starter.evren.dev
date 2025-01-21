import { defineStore } from "pinia";

export const useAppStore = defineStore({
  id: "app",
  state: () => ({
    loading: true
  }),
  actions: {
    togglePreloader(value) {
      console.log("togglePreloader", value);
      if (!value) {
        this.loading = !this.loading;
      } else {
        this.loading = value;
      }
    }
  }
});
