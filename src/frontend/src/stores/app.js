import { defineStore } from "pinia";

export const useAppStore = defineStore({
  id: "app",
  state: () => ({
    loading: true
  }),
  actions: {
    setPageLoader(value) {
      this.loading = value;
    }
  }
});
