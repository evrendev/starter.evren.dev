import { defineStore } from "pinia"

export const useAppStore = defineStore({
  id: "app",
  state: () => ({
    loading: true,
  }),
  actions: {
    togglePreloader() {
      this.loading = !this.loading
    },
  },
})
