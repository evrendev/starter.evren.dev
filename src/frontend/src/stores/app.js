import { defineStore } from "pinia";

export const useAppStore = defineStore("app", {
  state: () => ({
    loading: false
  }),
  actions: {
    setLoading(value) {
      this.loading = value;
    },
    async handlePageTransition(to) {
      const shouldShowLoader = to.meta.showLoader !== false;

      if (shouldShowLoader) {
        this.setLoading(true);
        await new Promise((resolve) => setTimeout(resolve, 300));
      }

      return true;
    },
    clearPageLoading() {
      this.setLoading(false);
    }
  }
});
