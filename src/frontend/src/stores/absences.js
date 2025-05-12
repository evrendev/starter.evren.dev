import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useAbsenceStore = defineStore("absence", {
  state: () => ({
    items: [],
    itemsLength: 0,
    reset: false
  }),
  actions: {
    async getItems() {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const response = await apiService.get("/absences", false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        appStore.setLoading(false);
      }
    },
    async delete(id) {
      this.reset = true;

      try {
        await apiService.delete(`/absences/${id}`);
      } finally {
        this.reset = false;
      }
    },
    async create(absence) {
      const response = await apiService.post("/absences", absence);
      return response;
    },
    async update(id, absence) {
      const response = await apiService.put(`/absences/${id}`, absence);
      return response;
    }
  }
});
