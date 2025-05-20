import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useAbsenceStore = defineStore("absence", {
  state: () => ({
    events: [],
    reset: false
  }),
  actions: {
    async getEvents() {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        this.events = await apiService.get("/absences", false);
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
