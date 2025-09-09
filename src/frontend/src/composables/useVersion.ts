import { useHttpClient } from "@/composables/useHttpClient";
import { useDateFormat } from "@vueuse/core";

export const useVersion = () => {
  const version = ref("");
  const buildTime = ref();
  onMounted(async () => {
    try {
      const { data } = await useHttpClient().get("/version");
      version.value = data?.version;
      buildTime.value = useDateFormat(data?.buildTime, "DD MMM YY HH:mm");
    } catch (error) {
      console.error("Error fetching version:", error);
    }
  });

  return {
    version,
    buildTime,
  };
};
