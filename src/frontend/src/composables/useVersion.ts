import { useHttpClient } from "@/composables/useHttpClient";

export const useVersion = () => {
  const version = ref("");
  onMounted(async () => {
    try {
      const { data } = await useHttpClient().get("/version");
      version.value = data?.version;
    } catch (error) {
      console.error("Error fetching version:", error);
    }
  });

  return {
    version,
  };
};
