<template>
  <v-overlay :model-value="loading" class="align-center justify-center" scrim="white" persistent>
    <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
  </v-overlay>
</template>

<script setup>
import { useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
const appStore = useAppStore();

const { loading } = storeToRefs(appStore);
</script>
