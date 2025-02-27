<template>
  <!-- Global loading overlay with blur effect -->
  <div v-if="pageLoading" class="page-loading-overlay">
    <v-progress-linear color="primary" indeterminate height="4" class="page-loading-bar"></v-progress-linear>
  </div>

  <!-- Original loader for full-page loading (kept for backward compatibility) -->
  <v-overlay :model-value="loading" class="align-center justify-center" scrim="white" persistent>
    <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
  </v-overlay>
</template>

<script setup>
import { useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
const appStore = useAppStore();

const { loading, pageLoading } = storeToRefs(appStore);
</script>

<style lang="scss" scoped>
.page-loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 9999;
  pointer-events: none;

  .page-loading-bar {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  }

  &::after {
    content: "";
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    backdrop-filter: blur(2px);
    background-color: rgba(255, 255, 255, 0.15);
    z-index: -1;
    pointer-events: none;
    transition:
      backdrop-filter 0.3s ease,
      background-color 0.3s ease;
  }
}
</style>
