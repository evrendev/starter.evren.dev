import { defineStore } from "pinia";

export const useAppStore = defineStore("app", {
  state: () => ({
    loading: false,
    pageLoading: false
  }),
  actions: {
    setLoading(value) {
      this.loading = value;
    },
    setPageLoading(value) {
      this.pageLoading = value;
    },
    async handlePageTransition(to) {
      // Check if the route requires loading state
      const shouldShowLoader = to.meta.showLoader !== false; // Default to true unless explicitly set to false

      if (shouldShowLoader) {
        this.setPageLoading(true);
        // Minimum loading time to prevent flashing
        await new Promise((resolve) => setTimeout(resolve, 300));
      }

      return true;
    },
    clearPageLoading() {
      this.setPageLoading(false);
    }
  }
});
