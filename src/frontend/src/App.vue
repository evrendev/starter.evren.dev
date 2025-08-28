<script setup lang="ts">
import AppNotification from "@/layouts/components/AppNotification.vue";
import Loader from "@/layouts/components/Loader.vue";
</script>

<template>
  <VApp>
    <Loader />
    <RouterView />

    <AppNotification />
  </VApp>
</template>
