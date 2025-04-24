import { ref, onMounted } from "vue";
import { apiService } from "@/utils/helpers";

export function useVersion() {
  const versionInfo = ref(null);

  onMounted(async () => {
    try {
      const data = await apiService.get("/version");
      versionInfo.value = data;
    } catch (e) {
      console.error("Versiyon bilgisi alınamadı", e);
    }
  });

  return { versionInfo };
}
