import { ref } from "vue";
import { defineStore } from "pinia";
import { set } from "@vueuse/core";

export const useAppStore = defineStore("app", () => {
  const loading = ref<boolean>(false);

  function setLoading(value: boolean) {
    loading.value = value;
  }

  return {
    loading,
    setLoading,
  };
});
