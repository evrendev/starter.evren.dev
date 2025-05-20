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
        const index = this.events.findIndex((event) => event.id === id);
        await apiService.delete(`/absences/${id}`);
        if (index !== -1) this.events.splice(index, 1);
      } finally {
        this.reset = false;
      }
    },
    async save(absence) {
      const response = await apiService.post("/absences", absence);
      absence.id = response;
      this.events.push(absence);
      return response;
    },
    async update(id, absence) {
      const response = await apiService.put(`/absences/${id}`, absence);
      return response;
    }
  }
});
