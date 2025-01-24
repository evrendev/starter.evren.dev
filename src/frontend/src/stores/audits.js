import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";

export const useAuditStore = defineStore({
  id: "audit",
  state: () => ({
    items: [],
    loading: false
  }),
  actions: {
    async load(query) {
      this.loading = true;
      this.items = await apiService.get(`/audits?${query}`);
      this.loading = false;
    }
  }
});
